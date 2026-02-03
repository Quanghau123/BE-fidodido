using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Domain.Enums.Game;
using FidoDino.Common;
using FidoDino.Common.Exceptions;
using FidoDino.Infrastructure.Data;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using FidoDino.Application.Interfaces;
using FidoDino.Application.DTOs.Game;
using FidoDino.Infrastructure.Redis;
using System.Text.Json;

namespace FidoDino.Application.Services
{
    public class PlayTurnService : IPlayTurnService
    {
        private readonly FidoDinoDbContext _db;
        private readonly IDatabase _redis;
        private readonly IGamePlayService _gamePlayService;
        private readonly IEffectService _effectService;
        private readonly TimeRangeType _defaultTimeRange;

        public PlayTurnService(
            FidoDinoDbContext db,
            IConnectionMultiplexer redis,
            IGamePlayService gamePlayService,
            IEffectService effectService,
            IConfiguration config)
        {
            _db = db;
            _redis = redis.GetDatabase();
            _gamePlayService = gamePlayService;
            _effectService = effectService;

            Enum.TryParse(config["Leaderboard:DefaultTimeRange"], true, out _defaultTimeRange);
            if (_defaultTimeRange == 0)
                _defaultTimeRange = TimeRangeType.Day;
        }

        /// <summary>
        /// Bắt đầu lượt chơi cho user, kiểm tra trạng thái hệ thống, session và hiệu ứng block.
        /// </summary>
        public async Task<StartTurnResultDto> StartTurnAsync(Guid userId, Guid sessionId)
        {
            var lockKey = $"game:lock:{userId}";
            if (!await _redis.StringSetAsync(lockKey, "1", TimeSpan.FromSeconds(5), When.NotExists))
                throw new ConflictException("User is already playing");

            try
            {
                if (await _redis.StringGetAsync("game:system_status") != "Active")
                    throw new ForbiddenException("System inactive");

                var cachedSessionId = await _redis.StringGetAsync($"game:session:user:{userId}");
                if (!cachedSessionId.HasValue || cachedSessionId != sessionId.ToString())
                    throw new NotFoundException("No active session");

                // Cập nhật hiệu ứng timed khi bắt đầu lượt chơi
                Console.WriteLine($"[LOG][PlayTurnService] StartTurnAsync: userId={userId}");
                await _effectService.UpdateTimedEffectsOnStartTurnAsync(userId);

                if (await _effectService.HasEffectAsync(userId, EffectType.BlockPlay))
                    throw new ForbiddenException("User is blocked");

                var (ice, shakeCount) = await _gamePlayService.StartTurnAsync(userId);

                var effectResult =
                    await _effectService.ApplyStartTurnEffectsAsync(userId, shakeCount);

                var turnCache = new TurnCacheDto
                {
                    SessionId = sessionId,
                    IceId = ice.IceId,
                    ShakeCount = effectResult.FinalShakeCount,
                    StartedAt = DateTime.UtcNow,
                    DoubleScore = effectResult.DoubleScore
                };

                await _redis.StringSetAsync(
                    $"game:turn:active:{userId}",
                    JsonSerializer.Serialize(turnCache),
                    TimeSpan.FromSeconds(60)
                );

                return new StartTurnResultDto
                {
                    IceId = ice.IceId,
                    IceType = ice.IceType.ToString(),
                    ShakeCount = effectResult.FinalShakeCount
                };
            }
            finally
            {
                await _redis.KeyDeleteAsync(lockKey);
            }
        }

        /// <summary>
        /// Kết thúc lượt chơi, tính điểm, cập nhật bảng xếp hạng, áp dụng hiệu ứng và trả về kết quả lượt chơi.
        /// </summary>
        public async Task<PlayTurnResultDto> EndTurnAsync(Guid userId)
        {
            var turnJson = await _redis.StringGetAsync($"game:turn:active:{userId}");
            if (!turnJson.HasValue)
                throw new NotFoundException("No active turn");

            var turn = JsonSerializer.Deserialize<TurnCacheDto>(turnJson!)!;


            // Tính thời gian thực sự chơi
            var playDurationSeconds = (int)(DateTime.UtcNow - turn.StartedAt).TotalSeconds;

            // Cập nhật hiệu ứng timed khi kết thúc lượt chơi
            Console.WriteLine($"[LOG][PlayTurnService] EndTurnAsync: userId={userId}, playDurationSeconds={playDurationSeconds}");
            await _effectService.UpdateTimedEffectsOnEndTurnAsync(userId, playDurationSeconds);

            var (reward, baseScore) = await _gamePlayService.EndTurnAsync(userId, turn.IceId);

            var scoreResult =
                await _effectService.ApplyEndTurnEffectsAsync(
                    baseScore,
                    turn.DoubleScore);

            var earnedScore = scoreResult.FinalScore;

            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var timeKey = LeaderboardTimeKeyHelper.GetTimeKey(_defaultTimeRange, now);

            var playTurn = new PlayTurn
            {
                PlayTurnId = Guid.NewGuid(),
                GameSessionId = turn.SessionId,
                IceId = turn.IceId,
                RewardId = reward.RewardId,
                ShakeCount = turn.ShakeCount,
                EarnedScore = earnedScore,
                PlayedAt = DateTime.UtcNow,
                TimeRange = _defaultTimeRange,
                TimeKey = timeKey
            };

            _db.PlayTurns.Add(playTurn);

            var state = await _db.LeaderboardStates
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.TimeRange == _defaultTimeRange &&
                    x.TimeKey == timeKey);

            if (state == null)
            {
                state = new LeaderboardState
                {
                    UserId = userId,
                    TimeRange = _defaultTimeRange,
                    TimeKey = timeKey,
                    TotalScore = earnedScore,
                    PlayCount = 1,
                    AchievedAt = DateTime.UtcNow,
                    StableRandom = Math.Abs($"{userId}{timeKey}".GetHashCode()) % 1000,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.LeaderboardStates.Add(state);
            }
            else
            {
                state.TotalScore += earnedScore;
                state.PlayCount++;
                state.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            //SortedSetIncrementAsync dùng để cộng thêm một giá trị vào score của một member trong Redis Sorted Set (tự tạo nếu chưa tồn tại)
            await _redis.SortedSetIncrementAsync(
                $"leaderboard:{_defaultTimeRange}:{timeKey}",
                userId.ToString(),
                earnedScore);

            // Apply effect
            var rewardEffectResult = await _effectService.ApplyRewardEffectAsync(userId, reward.EffectType);

            await _redis.KeyDeleteAsync($"game:turn:active:{userId}");

            return new PlayTurnResultDto
            {
                PlayTurnId = playTurn.PlayTurnId,
                IceId = turn.IceId,
                ShakeCount = turn.ShakeCount,
                RewardId = reward.RewardId,
                RewardName = reward.RewardName,
                EarnedScore = earnedScore,
                EffectType = rewardEffectResult.EffectType,
                DurationSeconds = rewardEffectResult.DurationSeconds,
            };
        }

    }
}
