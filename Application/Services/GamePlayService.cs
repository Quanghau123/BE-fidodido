using FidoDino.Infrastructure.Redis;
using FidoDino.Domain.Enums.Game;
using FidoDino.Common.Exceptions;
using FidoDino.Application.Interfaces;
using FidoDino.Application.DTOs.Game;

namespace FidoDino.Application.Services
{
    public class GamePlayService : IGamePlayService
    {
        private readonly IceCacheService _iceCache;
        private readonly IceRewardCacheService _iceRewardCache;
        private readonly RewardCacheService _rewardCache;
        private readonly IEffectService _effectService;
        private readonly StackExchange.Redis.IConnectionMultiplexer _multiplexer;

        public GamePlayService(
            IceCacheService iceCache,
            IceRewardCacheService iceRewardCache,
            RewardCacheService rewardCache,
            IEffectService effectService,
            StackExchange.Redis.IConnectionMultiplexer multiplexer)
        {
            _iceCache = iceCache;
            _iceRewardCache = iceRewardCache;
            _rewardCache = rewardCache;
            _effectService = effectService;
            _multiplexer = multiplexer;
        }

        public async Task<(IceResultDto ice, int shakeCount)> StartTurnAsync(Guid userId)
        {
            var iceList = await _iceCache.GetAllIcesAsync();
            var ice = RandomIceByProbability(iceList);

            int shakeCount = ice.ShakeCount;

            if (await _effectService.HasEffectAsync(userId, EffectType.Utility))
            {
                shakeCount = 0;
                await _effectService.ConsumeUtilityAsync(userId);
            }

            if (await _effectService.HasEffectAsync(userId, EffectType.SpeedBoost))
            {
                shakeCount = (int)Math.Ceiling(shakeCount * 0.5);
            }

            return (ice, shakeCount);
        }
        public async Task<(RewardResultDto reward, int earnedScore)> EndTurnAsync(Guid userId, Guid iceId)
        {
            var iceRewards = await _iceRewardCache.GetIceRewardsAsync(iceId);
            var iceReward = RandomRewardByProbability(iceRewards);

            var rewardRedis = await _rewardCache.GetRewardAsync(iceReward.RewardId)
                ?? throw new NotFoundException("Reward not found");

            // Tra cứu EffectType từ EffectId
            string effectTypeName = EffectType.None.ToString();
            if (rewardRedis.EffectId.HasValue && rewardRedis.EffectId.Value != Guid.Empty)
            {
                var effectCache = new EffectCacheService(_multiplexer);
                var effect = await effectCache.GetEffectAsync(rewardRedis.EffectId.Value);
                if (effect != null)
                    effectTypeName = effect.EffectType.ToString();
            }
            Console.WriteLine($"//////////////////////////////////////////////////");
            Console.WriteLine($"[DEBUG]  effectTypeName {effectTypeName} ");
            Console.WriteLine($"//////////////////////////////////////////////////");
            int earnedScore = rewardRedis.Score;
            if (await _effectService.HasEffectAsync(userId, EffectType.DoubleScore))
            {
                earnedScore *= 2;
            }

            var reward = new RewardResultDto
            {
                RewardId = rewardRedis.RewardId,
                RewardName = rewardRedis.RewardName,
                Score = rewardRedis.Score,
                EffectType = Enum.TryParse<EffectType>(effectTypeName, out var effectType) ? effectType : EffectType.None
            };

            return (reward, earnedScore);
        }

        private IceResultDto RandomIceByProbability(List<IceResultDto> iceList)
        {
            if (!iceList.Any())
                throw new NotFoundException("No ice available");

            double roll = Random.Shared.NextDouble();
            double sum = 0;

            foreach (var ice in iceList.OrderBy(i => i.Probability))
            {
                sum += ice.Probability;
                if (roll < sum)
                    return ice;
            }

            return iceList.Last();
        }

        private IceRewardCacheService.IceRewardRedisDto RandomRewardByProbability(
            List<IceRewardCacheService.IceRewardRedisDto> rewards)
        {
            if (!rewards.Any())
                throw new NotFoundException("No rewards for ice");

            double roll = Random.Shared.NextDouble();
            double sum = 0;

            foreach (var r in rewards.OrderBy(r => r.Probability))
            {
                sum += r.Probability;
                if (roll < sum)
                    return r;
            }

            return rewards.Last();
        }
    }
}
