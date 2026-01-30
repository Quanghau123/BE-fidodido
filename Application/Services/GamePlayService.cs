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
        //IConnectionMultiplexer là đối tượng duy nhất dùng chung trong app để quản lý toàn bộ kết nối Redis
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

        // Bắt đầu lượt chơi, trả về loại ice và số lần lắc cho user.
        public async Task<(IceResultDto ice, int shakeCount)> StartTurnAsync(Guid userId)
        {
            var iceList = await _iceCache.GetAllIcesAsync();
            var ice = RandomIceByProbability(iceList);

            int shakeCount = ice.ShakeCount;

            if (await _effectService.GetUtilityRemainAsync(userId) > 0)
            {
                shakeCount = 0;
                await _effectService.ConsumeUtilityAsync(userId);
            }

            if (await _effectService.HasEffectAsync(userId, EffectType.SpeedBoost))
            {
                //Math.Ceiling làm tròn số lên số nguyên gần nhất
                shakeCount = (int)Math.Ceiling(shakeCount * 0.5);
            }

            return (ice, shakeCount);
        }

        // Kết thúc lượt chơi, trả về phần thưởng và điểm số đạt được.
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
                //_multiplexer được dùng để tạo kết nối Redis cho EffectCacheService, từ đó lấy thông tin effect từ Redis theo EffectId
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

        // Chọn ngẫu nhiên một ice dựa trên xác suất.
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

        // Chọn ngẫu nhiên một phần thưởng dựa trên xác suất.
        private IceRewardRedisDto RandomRewardByProbability(List<IceRewardRedisDto> rewards)
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
