using System.Text.Json;
using FidoDino.Application.DTOs.Game;
using StackExchange.Redis;

namespace FidoDino.Infrastructure.Redis
{
    public class EffectCacheService
    {
        private readonly IDatabase _redis;

        public EffectCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        // ===================== TIMED EFFECTS (NO TTL) =====================
        public async Task<List<TimedEffectStateDto>> GetTimedEffectsAsync(Guid userId)
        {
            var json = await _redis.StringGetAsync($"game:timed_effects:{userId}");
            if (!json.HasValue) return new List<TimedEffectStateDto>();
            return JsonSerializer.Deserialize<List<TimedEffectStateDto>>(json!) ?? new List<TimedEffectStateDto>();
        }

        public Task SaveTimedEffectsAsync(Guid userId, List<TimedEffectStateDto> effects)
        {
            var json = JsonSerializer.Serialize(effects);
            return _redis.StringSetAsync($"game:timed_effects:{userId}", json);
        }

        // Lấy effect từ Redis theo EffectId
        public async Task<EffectRedisDto?> GetEffectAsync(Guid effectId)
        {
            var json = await _redis.HashGetAsync("game:effect:all", effectId.ToString());
            if (json.IsNullOrEmpty) return null;
            return System.Text.Json.JsonSerializer.Deserialize<EffectRedisDto>(json!);
        }

        /* ===================== EFFECT CHUNG ===================== */

        public Task<bool> HasEffect(Guid userId, string effectType)
        {
            return _redis.KeyExistsAsync($"game:effect:{userId}:{effectType}");
        }

        public Task SetEffect(Guid userId, string effectType, int seconds)
        {
            // Deprecated for timed effect, use SaveTimedEffectsAsync instead
            return Task.CompletedTask;
        }

        public Task RemoveEffect(Guid userId, string effectType)
        {
            // Deprecated for timed effect, use SaveTimedEffectsAsync instead
            return Task.CompletedTask;
        }

        public async Task<int?> GetEffectRemainSeconds(Guid userId, string effectType)
        {
            // Deprecated for timed effect, use GetTimedEffectsAsync instead
            var effects = await GetTimedEffectsAsync(userId);
            var effect = effects.FirstOrDefault(e => e.EffectType.ToString() == effectType);
            return effect?.RemainingSeconds;
        }

        // ...existing code...

        /* ===================== UTILITY ===================== */

        public async Task<int> GetUtilityRemain(Guid userId)
        {
            var val = await _redis.StringGetAsync($"effect:utility:{userId}");
            return val.HasValue ? (int)val : 0;
        }

        public Task SetUtilityRemain(Guid userId, int remain)
        {
            return _redis.StringSetAsync(
                $"effect:utility:{userId}",
                remain,
                TimeSpan.FromMinutes(10)
            );
        }

        public Task DecrementUtility(Guid userId)
        {
            return _redis.StringDecrementAsync($"effect:utility:{userId}");
        }

        /* ===================== SPEED ===================== */

        public async Task<bool> HasSpeedBoost(Guid userId)
        {
            var val = await _redis.StringGetAsync($"effect:speed:{userId}");
            return val.HasValue && val == "1";
        }

        public Task SetSpeedBoost(Guid userId, int seconds)
        {
            return _redis.StringSetAsync(
                $"effect:speed:{userId}",
                1,
                TimeSpan.FromSeconds(seconds)
            );
        }

        public Task RemoveSpeedBoost(Guid userId)
        {
            return _redis.KeyDeleteAsync($"effect:speed:{userId}");
        }

        /* ===================== PENALTY ===================== */

        public async Task<bool> HasPenalty(Guid userId)
        {
            var val = await _redis.StringGetAsync($"effect:penalty:{userId}");
            return val.HasValue && val == "1";
        }

        public Task SetPenalty(Guid userId, int seconds)
        {
            return _redis.StringSetAsync(
                $"effect:penalty:{userId}",
                1,
                TimeSpan.FromSeconds(seconds)
            );
        }

        public Task RemovePenalty(Guid userId)
        {
            return _redis.KeyDeleteAsync($"effect:penalty:{userId}");
        }
    }
}
