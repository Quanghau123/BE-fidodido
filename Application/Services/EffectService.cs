using System.ComponentModel.DataAnnotations;
using FidoDino.Application.Interface;
using FidoDino.Common.Exceptions;
using StackExchange.Redis;

namespace FidoDino.Application.Services
{
    public class EffectService : IEffectService
    {
        private readonly IDatabase _redis;

        public EffectService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        /// <summary>
        /// Kiểm tra người chơi có hiệu ứng (effect) cụ thể hay không.
        /// </summary>
        public async Task<bool> HasEffectAsync(Guid userId, string effectType)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (string.IsNullOrWhiteSpace(effectType))
                throw new ArgumentException("EffectType is required");
            return await _redis.KeyExistsAsync($"game:effect:{userId}:{effectType}");
        }

        /// <summary>
        /// Thiết lập hiệu ứng cho người chơi với thời gian tồn tại (giây).
        /// </summary>
        public async Task SetEffectAsync(Guid userId, string effectType, int durationSeconds)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (string.IsNullOrWhiteSpace(effectType))
                throw new ArgumentException("EffectType is required");
            if (durationSeconds <= 0)
                throw new ValidationException("Duration must be greater than 0");
            await _redis.StringSetAsync($"game:effect:{userId}:{effectType}", durationSeconds, TimeSpan.FromSeconds(durationSeconds));
        }

        /// <summary>
        /// Xóa hiệu ứng của người chơi khỏi Redis.
        /// </summary>
        public async Task RemoveEffectAsync(Guid userId, string effectType)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (string.IsNullOrWhiteSpace(effectType))
                throw new ArgumentException("EffectType is required");
            await _redis.KeyDeleteAsync($"game:effect:{userId}:{effectType}");
        }

        /// <summary>
        /// Lấy thời gian hiệu ứng còn lại của người chơi (tính bằng giây).
        /// </summary>
        public async Task<int> GetEffectDurationAsync(Guid userId, string effectType)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (string.IsNullOrWhiteSpace(effectType))
                throw new ArgumentException("EffectType is required");
            var value = await _redis.StringGetAsync($"game:effect:{userId}:{effectType}");
            if (!value.HasValue)
                throw new NotFoundException("Effect not found for user");
            return int.Parse(value!);
        }
    }
}
