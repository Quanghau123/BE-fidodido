using FidoDino.Application.DTOs.Game;
using StackExchange.Redis;
using System.Text.Json;

namespace FidoDino.Infrastructure.Redis
{
    public class IceRewardCacheService
    {
        private readonly IDatabase _redis;
        public IceRewardCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<List<IceRewardRedisDto>> GetIceRewardsAsync(Guid iceId)
        {
            var json = await _redis.StringGetAsync($"game:ice_reward:{iceId}");
            if (json.IsNullOrEmpty) return new List<IceRewardRedisDto>();
            return JsonSerializer.Deserialize<List<IceRewardRedisDto>>(json!) ?? new List<IceRewardRedisDto>();
        }
    }
}
