using StackExchange.Redis;

namespace FidoDino.Infrastructure.Redis
{
    public class EffectCacheService
    {
        private readonly IDatabase _redis;
        public EffectCacheService(IDatabase redis)
        {
            _redis = redis;
        }
        /// <summary>
        /// Lấy số lượng hiệu ứng utility còn lại của người chơi từ Redis.
        /// </summary>
        public async Task<int> GetUtilityRemain(Guid userId)
        {
            var val = await _redis.StringGetAsync($"effect:utility:{userId}");
            return val.HasValue ? (int)val : 0;
        }
        /// <summary>
        /// Thiết lập số lượng hiệu ứng utility còn lại cho người chơi trong Redis (tồn tại 10 phút).
        /// </summary>
        public async Task SetUtilityRemain(Guid userId, int remain)
        {
            await _redis.StringSetAsync($"effect:utility:{userId}", remain, TimeSpan.FromMinutes(10));
        }
        /// <summary>
        /// Giảm số lượng hiệu ứng utility của người chơi đi 1 trong Redis.
        /// </summary>
        public async Task DecrementUtility(Guid userId)
        {
            await _redis.StringDecrementAsync($"effect:utility:{userId}");
        }
        /// <summary>
        /// Kiểm tra người chơi có hiệu ứng tăng tốc (speed boost) hay không.
        /// </summary>
        public async Task<bool> HasSpeedBoost(Guid userId)
        {
            var val = await _redis.StringGetAsync($"effect:speed:{userId}");
            return val.HasValue && val == "1";
        }
        /// <summary>
        /// Thiết lập hiệu ứng tăng tốc cho người chơi trong một khoảng thời gian (giây).
        /// </summary>
        public async Task SetSpeedBoost(Guid userId, int seconds)
        {
            await _redis.StringSetAsync($"effect:speed:{userId}", 1, TimeSpan.FromSeconds(seconds));
        }
        /// <summary>
        /// Xóa hiệu ứng tăng tốc của người chơi khỏi Redis.
        /// </summary>
        public async Task RemoveSpeedBoost(Guid userId)
        {
            await _redis.KeyDeleteAsync($"effect:speed:{userId}");
        }
        /// <summary>
        /// Kiểm tra người chơi có hiệu ứng phạt (penalty) hay không.
        /// </summary>
        public async Task<bool> HasPenalty(Guid userId)
        {
            var val = await _redis.StringGetAsync($"effect:penalty:{userId}");
            return val.HasValue && val == "1";
        }
        /// <summary>
        /// Thiết lập hiệu ứng phạt cho người chơi trong một khoảng thời gian (giây).
        /// </summary>
        public async Task SetPenalty(Guid userId, int seconds)
        {
            await _redis.StringSetAsync($"effect:penalty:{userId}", 1, TimeSpan.FromSeconds(seconds));
        }
        /// <summary>
        /// Xóa hiệu ứng phạt của người chơi khỏi Redis.
        /// </summary>
        public async Task RemovePenalty(Guid userId)
        {
            await _redis.KeyDeleteAsync($"effect:penalty:{userId}");
        }
    }
}