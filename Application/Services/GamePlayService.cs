using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Application.Interfaces;
using FidoDino.Application.DTOs.Game;
using FidoDino.Domain.Enums.Game;
using FidoDino.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using FidoDino.Domain.Entities.Leaderboard;
using FidoDino.Common;

namespace FidoDino.Application.Services
{
    public class GamePlayService : IGamePlayService
    {
        private readonly IGameSessionRepository _sessionRepo;
        private readonly IPlayTurnRepository _turnRepo;
        private readonly IIceRepository _iceRepo;
        private readonly Infrastructure.Redis.EffectCacheService _effectCache;
        private readonly Domain.Interfaces.Leaderboard.ILeaderboardStateRepository _leaderboardStateRepo;
        private readonly Infrastructure.Data.FidoDinoDbContext _db;
        private readonly TimeRangeType _defaultTimeRange;
        public GamePlayService(
            IGameSessionRepository sessionRepo,
            IPlayTurnRepository turnRepo,
            IIceRepository iceRepo,
            Infrastructure.Redis.EffectCacheService effectCache,
            Domain.Interfaces.Leaderboard.ILeaderboardStateRepository leaderboardStateRepo,
            Infrastructure.Data.FidoDinoDbContext db,
            IConfiguration configuration
        )
        {
            _sessionRepo = sessionRepo;
            _turnRepo = turnRepo;
            _iceRepo = iceRepo;
            _effectCache = effectCache;
            _leaderboardStateRepo = leaderboardStateRepo;
            _db = db;
            var configValue = configuration["Leaderboard:DefaultTimeRange"] ?? "Day";
            if (!Enum.TryParse<TimeRangeType>(configValue, true, out var parsed))
                parsed = TimeRangeType.Day;
            _defaultTimeRange = parsed;
        }

        /// <summary>
        /// Random loại đá (Ice) cho người chơi và áp dụng hiệu ứng nếu có.
        /// </summary>
        public async Task<IceResultDto> RandomIceAndApplyEffect(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            var rand = new Random();
            //0.0 <= value < 1.0
            double r = rand.NextDouble();
            IceType iceType;
            if (r < 0.2) iceType = IceType.Large;
            else if (r < 0.5) iceType = IceType.Medium;
            else iceType = IceType.Small;

            var ices = await _iceRepo.GetAllAsync();
            var ice = ices.FirstOrDefault(i => i.IceType == iceType);
            if (ice == null)
                throw new NotFoundException($"No ice found for type: {iceType}");

            int shakeCount = ice.RequiredShake;
            //Xử lý hiệu ứng Utility (Kính lúp - Support Effect)
            var utilityRemain = await _effectCache.GetUtilityRemain(userId);
            if (utilityRemain > 0)
            {
                shakeCount = 0;
                await _effectCache.DecrementUtility(userId);
            }

            //Xử lý hiệu ứng Speedboost (Ván trượt)
            if (await _effectCache.HasSpeedBoost(userId))
                // Giảm 50% số lần lắc cần thiết (làm tròn lên)
                shakeCount = (int)Math.Ceiling(shakeCount * 0.5);

            return new IceResultDto
            {
                IceId = ice.IceId,
                IceType = ice.IceType,
                ShakeCount = shakeCount
            };
        }

        /// <summary>
        /// Random phần thưởng cho lượt chơi dựa trên iceId và xác suất từng phần thưởng.
        /// </summary>
        public async Task<RewardResultDto> RandomReward(Guid iceId)
        {
            if (iceId == Guid.Empty)
                throw new ArgumentException("IceId is required");
            var ice = await _iceRepo.GetByIdAsync(iceId);
            if (ice == null)
                throw new NotFoundException($"Ice not found with id: {iceId}");
            var iceRewards = ice.IceRewards;
            if (iceRewards == null || !iceRewards.Any())
                throw new NotFoundException($"No IceReward found for iceId: {iceId}");
            double r = new Random().NextDouble();
            double sum = 0;
            foreach (var ir in iceRewards.OrderBy(x => x.Probability))
            {
                sum += ir.Probability;
                if (r < sum)
                {
                    var reward = ir.Reward;
                    if (reward == null)
                        throw new NotFoundException($"Reward not found for IceRewardId: {ir.IceRewardId}");
                    if (reward.Effect != null)
                    {
                        switch (reward.Effect.EffectType)
                        {
                            case EffectType.Utility:
                                await _effectCache.SetUtilityRemain(reward.RewardId, 3);
                                break;
                            case EffectType.SpeedBoost:
                                await _effectCache.SetSpeedBoost(reward.RewardId, reward.Effect.DurationSeconds);
                                break;
                            case EffectType.BlockPlay:
                                // Nếu có SetPenalty thì dùng
                                break;
                        }
                    }
                    return new RewardResultDto
                    {
                        RewardId = reward.RewardId,
                        RewardName = reward.RewardName,
                        Score = reward.Score,
                        EffectType = reward.Effect?.EffectType.ToString()
                    };
                }
            }
            throw new NotFoundException($"No reward selected for iceId: {iceId}. Check probabilities and data.");
        }

        /// <summary>
        /// Lưu lượt chơi (PlayTurn), cập nhật điểm số và trạng thái bảng xếp hạng cho người chơi.
        /// </summary>
        public async Task<PlayTurnResultDto> CompletePlayTurn(Guid sessionId, Guid iceId, Guid rewardId, int shakeCount, int earnedScore)
        {
            if (sessionId == Guid.Empty)
                throw new ArgumentException("SessionId is required");
            if (iceId == Guid.Empty)
                throw new ArgumentException("IceId is required");
            if (rewardId == Guid.Empty)
                throw new ArgumentException("RewardId is required");
            if (shakeCount < 0)
                throw new ValidationException("ShakeCount must be >= 0");
            if (earnedScore < 0)
                throw new ValidationException("EarnedScore must be >= 0");
            using var tx = await _db.Database.BeginTransactionAsync();
            var playTurn = new PlayTurn
            {
                PlayTurnId = Guid.NewGuid(),
                GameSessionId = sessionId,
                IceId = iceId,
                RewardId = rewardId,
                ShakeCount = shakeCount,
                EarnedScore = earnedScore,
                PlayedAt = DateTime.UtcNow
            };
            await _turnRepo.AddAsync(playTurn);

            var session = await _sessionRepo.GetByIdAsync(sessionId);
            if (session == null)
            {
                await tx.RollbackAsync();
                throw new NotFoundException("Session not found");
            }

            session.TotalScore += earnedScore;
            await _sessionRepo.UpdateAsync(session);

            var now = DateTime.UtcNow;
            var timeKey = LeaderboardTimeKeyHelper.GetTimeKey(_defaultTimeRange, now);
            var state = await _leaderboardStateRepo.GetByUserAndTimeAsync(session.UserId, _defaultTimeRange, timeKey);
            if (state == null)
            {
                state = new LeaderboardState
                {
                    UserId = session.UserId,
                    TimeRange = _defaultTimeRange,
                    TimeKey = timeKey,
                    TotalScore = earnedScore,
                    PlayCount = 1,
                    AchievedAt = now,
                    StableRandom = Math.Abs($"{session.UserId}{(int)_defaultTimeRange}{timeKey}".GetHashCode()) % 1000,
                    UpdatedAt = now
                };
            }
            else
            {
                state.TotalScore += earnedScore;
                state.PlayCount++;
                state.UpdatedAt = now;
            }
            await _leaderboardStateRepo.AddOrUpdateAsync(state);

            await tx.CommitAsync();
            return new PlayTurnResultDto
            {
                PlayTurnId = playTurn.PlayTurnId,
                EarnedScore = playTurn.EarnedScore
            };
        }
    }
}