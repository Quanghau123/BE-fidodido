using StackExchange.Redis;
using System.Text.Json;

namespace FidoDino.Infrastructure.Redis
{
    public class RewardCacheService
    {
        private readonly IDatabase _redis;
        public RewardCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<RewardRedisDto?> GetRewardAsync(Guid rewardId)
        {
            var json = await _redis.HashGetAsync("game:reward:all", rewardId.ToString());
            if (json.IsNullOrEmpty) return null;
            return JsonSerializer.Deserialize<RewardRedisDto>(json!);
        }

        public class RewardRedisDto
        {
            public Guid RewardId { get; set; }
            public Guid? EffectId { get; set; }
            public string RewardName { get; set; } = string.Empty;
            public int Score { get; set; }
        }
    }
}
