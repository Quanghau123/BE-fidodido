using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Entities.Leaderboard;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using FidoDino.Infrastructure.Data;
using FidoDino.Application.Interface;
using FidoDino.Application.DTOs.Game;
using FidoDino.Common.Exceptions;
using FidoDino.Domain.Enums.Game;
using FidoDino.Common;

namespace FidoDino.Application.Services
{
    public class PlayTurnService : IPlayTurnService
    {
        private readonly FidoDinoDbContext _db;
        private readonly IDatabase _redis;
        private readonly IEffectService _effectService;
        private readonly TimeRangeType _defaultTimeRange;

        public PlayTurnService(FidoDinoDbContext db, IConnectionMultiplexer redis, IEffectService effectService, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _db = db;
            _redis = redis.GetDatabase();
            _effectService = effectService;
            var configValue = configuration["Leaderboard:DefaultTimeRange"] ?? "Day";
            if (!Enum.TryParse<TimeRangeType>(configValue, true, out var parsed))
                parsed = TimeRangeType.Day;
            _defaultTimeRange = parsed;
        }

        /// <summary>
        /// Xử lý một lượt chơi của người dùng: random ice, random reward, áp dụng hiệu ứng, cập nhật điểm và leaderboard.
        /// </summary>
        public async Task<PlayTurnResultDto> PlayTurnAsync(Guid userId, Guid sessionId)
        {
            // Lock Redis để đảm bảo 1 user chỉ chơi 1 lượt tại 1 thời điểm
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (sessionId == Guid.Empty)
                throw new ArgumentException("SessionId is required");
            var lockKey = $"game:lock:{userId}";
            var lockAcquired = await _redis.StringSetAsync(lockKey, "1", TimeSpan.FromSeconds(5), when: When.NotExists);
            if (!lockAcquired)
                throw new ConflictException("User is already playing a turn.");

            try
            {
                // Check system status
                var status = await _redis.StringGetAsync("game:system_status");
                if (status != "Active")
                    throw new ForbiddenException("System is not active.");

                // Check session
                var session = await _db.GameSessions.Include(s => s.PlayTurns).FirstOrDefaultAsync(s => s.GameSessionId == sessionId && s.UserId == userId && s.EndTime == null);
                if (session == null)
                    throw new NotFoundException("No active session.");

                // Check penalty effect (BlockPlay)
                if (await _effectService.HasEffectAsync(userId, "BlockPlay"))
                    throw new ForbiddenException("User is currently blocked from playing.");

                // Random Ice theo xác suất
                var iceList = await _db.Ices.ToListAsync();
                var ice = RandomIceByProbability(iceList);

                // Xử lý hiệu ứng Utility (AutoBreakIce)
                int shakeCount = ice.RequiredShake;
                if (await _effectService.HasEffectAsync(userId, "Utility"))
                {
                    shakeCount = 0;
                    // Giảm số lượt utility còn lại
                    var remain = await _effectService.GetEffectDurationAsync(userId, "Utility");
                    if (remain > 1)
                        await _effectService.SetEffectAsync(userId, "Utility", remain - 1);
                    else
                        await _effectService.RemoveEffectAsync(userId, "Utility");
                }

                // Xử lý hiệu ứng SpeedBoost
                if (await _effectService.HasEffectAsync(userId, "SpeedBoost"))
                {
                    shakeCount = (int)(shakeCount * 0.7); // ví dụ giảm 30%
                }

                // Random Reward theo xác suất
                var iceRewards = await _db.IceRewards.Where(r => r.IceId == ice.IceId).ToListAsync();
                var reward = RandomRewardByProbability(iceRewards, await _db.Rewards.ToListAsync());

                // Xử lý hiệu ứng DoubleScore
                int earnedScore = reward.Score;
                if (await _effectService.HasEffectAsync(userId, "DoubleScore"))
                {
                    earnedScore *= 2;
                }

                // Create PlayTurn
                var playTurn = new PlayTurn
                {
                    GameSessionId = sessionId,
                    IceId = ice.IceId,
                    RewardId = reward.RewardId,
                    ShakeCount = shakeCount,
                    EarnedScore = earnedScore,
                    PlayedAt = DateTime.UtcNow
                };
                _db.PlayTurns.Add(playTurn);
                session.TotalScore += earnedScore;
                await _db.SaveChangesAsync();

                // Update LeaderboardState
                var timeRange = _defaultTimeRange;
                var now = DateTime.UtcNow;
                var timeKey = LeaderboardTimeKeyHelper.GetTimeKey(timeRange, now);
                var leaderboardState = await _db.LeaderboardStates.FirstOrDefaultAsync(l => l.UserId == userId && l.TimeRange == timeRange && l.TimeKey == timeKey);
                if (leaderboardState == null)
                {
                    leaderboardState = new LeaderboardState
                    {
                        UserId = userId,
                        TimeRange = timeRange,
                        TimeKey = timeKey,
                        TotalScore = earnedScore,
                        PlayCount = 1,
                        AchievedAt = now,
                        StableRandom = Math.Abs($"{userId}{(int)timeRange}{timeKey}".GetHashCode()) % 1000,
                        UpdatedAt = now
                    };
                    _db.LeaderboardStates.Add(leaderboardState);
                }
                else
                {
                    leaderboardState.TotalScore += earnedScore;
                    leaderboardState.PlayCount++;
                    leaderboardState.UpdatedAt = now;
                }
                await _db.SaveChangesAsync();

                // Create ScoreEvent
                var scoreEvent = new ScoreEvent
                {
                    UserId = userId,
                    GameSessionId = sessionId,
                    ScoreDelta = earnedScore,
                    TimeRange = timeRange,
                    CreatedAt = now,
                    AppliedToRedis = false
                };
                _db.ScoreEvents.Add(scoreEvent);
                await _db.SaveChangesAsync();

                // Update Redis leaderboard
                var leaderboardKey = $"leaderboard:{timeRange}:{timeKey}";
                await _redis.SortedSetIncrementAsync(leaderboardKey, userId.ToString(), earnedScore);
                scoreEvent.AppliedToRedis = true;
                await _db.SaveChangesAsync();

                // Xử lý hiệu ứng mới từ reward (nếu có)
                if (reward.Effect != null && reward.Effect.EffectType != EffectType.None)
                {
                    await _effectService.SetEffectAsync(userId, reward.Effect.EffectType.ToString(), reward.Effect.DurationSeconds);
                }

                return new PlayTurnResultDto
                {
                    IceId = ice.IceId,
                    IceName = ice.IceType.ToString(),
                    ShakeCount = shakeCount,
                    RewardId = reward.RewardId,
                    RewardName = reward.RewardName,
                    EarnedScore = earnedScore,
                    EffectInfo = reward.Effect?.EffectType.ToString() ?? "",
                    EffectDuration = reward.Effect?.DurationSeconds ?? 0
                };
            }
            finally
            {
                // Remove play lock in Redis
                await _redis.KeyDeleteAsync(lockKey);
            }
        }

        /// <summary>
        /// Random chọn loại đá (Ice) dựa trên xác suất từng loại.
        /// </summary>
        private Ice RandomIceByProbability(List<Ice> iceList)
        {
            // Random theo xác suất
            if (iceList == null || iceList.Count == 0)
                throw new NotFoundException("No ice available for random selection.");
            var rnd = new Random();
            double roll = rnd.NextDouble();
            double cumulative = 0;
            foreach (var ice in iceList.OrderBy(x => x.Probability))
            {
                cumulative += ice.Probability;
                if (roll < cumulative)
                    return ice;
            }
            return iceList.Last();
        }

        /// <summary>
        /// Random chọn phần thưởng dựa trên xác suất từng phần thưởng.
        /// </summary>
        private Reward RandomRewardByProbability(List<IceReward> iceRewards, List<Reward> rewards)
        {
            if (iceRewards == null || iceRewards.Count == 0)
                throw new NotFoundException("No ice rewards available for random selection.");
            if (rewards == null || rewards.Count == 0)
                throw new NotFoundException("No rewards available for random selection.");
            var rnd = new Random();
            double roll = rnd.NextDouble();
            double cumulative = 0;
            foreach (var ir in iceRewards.OrderBy(x => x.Probability))
            {
                cumulative += ir.Probability;
                if (roll < cumulative)
                {
                    var found = rewards.FirstOrDefault(r => r.RewardId == ir.RewardId);
                    if (found == null)
                        throw new NotFoundException($"Reward not found for IceReward: {ir.RewardId}");
                    return found;
                }
            }
            var lastIceReward = iceRewards.Last();
            var lastReward = rewards.FirstOrDefault(r => r.RewardId == lastIceReward.RewardId);
            if (lastReward == null)
                throw new NotFoundException($"Reward not found for last IceReward: {lastIceReward.RewardId}");
            return lastReward;
        }
    }
}
