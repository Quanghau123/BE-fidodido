using System.ComponentModel.DataAnnotations;
using FidoDino.Application.DTOs.Game;
using FidoDino.Application.Interfaces;
using FidoDino.Common.Exceptions;
using FidoDino.Domain.Enums.Game;
using FidoDino.Infrastructure.Redis;

namespace FidoDino.Application.Services
{
    public class EffectService : IEffectService
    {
        private readonly EffectCacheService _cache;

        public EffectService(EffectCacheService cache)
        {
            _cache = cache;
        }

        public async Task<List<ActiveEffectDto>> GetAllActiveEffectsAsync(Guid userId)
        {
            var effects = new List<ActiveEffectDto>();

            // Timed effects (không gồm BlockPlay)
            var timedEffects = await GetActiveTimedEffectsAsync(userId);
            effects.AddRange(timedEffects.Select(e => new ActiveEffectDto
            {
                EffectType = e.EffectType.ToString(),
                Duration = e.RemainingSeconds
            }));

            // BlockPlay
            if (await HasEffectAsync(userId, EffectType.BlockPlay))
            {
                var duration = await GetEffectDurationAsync(userId, EffectType.BlockPlay);
                effects.Add(new ActiveEffectDto { EffectType = "BlockPlay", Duration = duration });
            }

            // Utility
            var utilityCount = await GetUtilityRemainAsync(userId);
            if (utilityCount > 0)
            {
                effects.Add(new ActiveEffectDto { EffectType = "AutoBreakIce", Duration = utilityCount });
            }

            return effects;
        }

        public async Task<bool> HasEffectAsync(Guid userId, EffectType effectType)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (effectType == EffectType.None)
                throw new ArgumentException("EffectType is required");

            if (effectType == EffectType.AutoBreakIce)
                return await _cache.GetUtilityRemain(userId) > 0;

            // BlockPlay: dùng TTL Redis, không lưu vào timed effect chung
            if (effectType == EffectType.BlockPlay)
                return await _cache.HasEffect(userId, effectType.ToString());

            // Timed effect khác
            var effects = await _cache.GetTimedEffectsAsync(userId);
            var effect = effects.FirstOrDefault(e => e.EffectType == effectType);
            if (effect == null) return false;
            if (effect.LastActiveAt.HasValue)
            {
                var elapsed = (int)(DateTime.UtcNow - effect.LastActiveAt.Value).TotalSeconds;
                effect.RemainingSeconds = Math.Max(0, effect.RemainingSeconds - elapsed);
            }
            return effect.RemainingSeconds > 0;
        }

        public async Task SetEffectAsync(Guid userId, EffectType effectType, int durationSeconds)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (effectType == EffectType.None)
                throw new ArgumentException("EffectType is required");
            if (durationSeconds <= 0)
                throw new ValidationException("Duration must be greater than 0");

            if (effectType == EffectType.AutoBreakIce)
            {
                await _cache.SetUtilityRemain(userId, durationSeconds);
                return;
            }

            // BlockPlay: dùng TTL Redis
            if (effectType == EffectType.BlockPlay)
            {
                await _cache.SetEffect(userId, effectType.ToString(), durationSeconds);
                return;
            }

            // Timed effect khác
            var effects = await _cache.GetTimedEffectsAsync(userId);
            var effect = effects.FirstOrDefault(e => e.EffectType == effectType);
            if (effect != null)
            {
                effect.RemainingSeconds = durationSeconds;
                effect.LastActiveAt = null;
            }
            else
            {
                effects.Add(new TimedEffectStateDto
                {
                    EffectType = effectType,
                    RemainingSeconds = durationSeconds,
                    LastActiveAt = null
                });
            }
            await _cache.SaveTimedEffectsAsync(userId, effects);
        }

        public async Task RemoveEffectAsync(Guid userId, EffectType effectType)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (effectType == EffectType.None)
                throw new ArgumentException("EffectType is required");

            if (effectType == EffectType.AutoBreakIce)
            {
                await _cache.SetUtilityRemain(userId, 0);
                return;
            }

            // BlockPlay: xóa TTL Redis
            if (effectType == EffectType.BlockPlay)
            {
                await _cache.RemoveEffect(userId, effectType.ToString());
                return;
            }

            var effects = await _cache.GetTimedEffectsAsync(userId);
            effects.RemoveAll(e => e.EffectType == effectType);
            await _cache.SaveTimedEffectsAsync(userId, effects);
        }

        public async Task<int> GetEffectDurationAsync(Guid userId, EffectType effectType)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (effectType == EffectType.None)
                throw new ArgumentException("EffectType is required");

            if (effectType == EffectType.AutoBreakIce)
                return await _cache.GetUtilityRemain(userId);

            // BlockPlay: lấy TTL Redis
            if (effectType == EffectType.BlockPlay)
            {
                var ttl = await _cache.GetEffectRemainSeconds(userId, effectType.ToString());
                if (ttl == null)
                    throw new NotFoundException("Effect not found for user");
                return ttl.Value;
            }

            var effects = await _cache.GetTimedEffectsAsync(userId);
            var effect = effects.FirstOrDefault(e => e.EffectType == effectType);
            if (effect == null)
                throw new NotFoundException("Effect not found for user");
            if (effect.LastActiveAt.HasValue)
            {
                var elapsed = (int)(DateTime.UtcNow - effect.LastActiveAt.Value).TotalSeconds;
                effect.RemainingSeconds = Math.Max(0, effect.RemainingSeconds - elapsed);
            }
            return effect.RemainingSeconds;
        }

        public async Task<List<TimedEffectStateDto>> GetActiveTimedEffectsAsync(Guid userId)
        {
            var effects = await _cache.GetTimedEffectsAsync(userId);
            var now = DateTime.UtcNow;
            foreach (var effect in effects)
            {
                if (effect.LastActiveAt.HasValue)
                {
                    var elapsed = (int)(now - effect.LastActiveAt.Value).TotalSeconds;
                    effect.RemainingSeconds = Math.Max(0, effect.RemainingSeconds - elapsed);
                }
            }
            // Không trả về BlockPlay trong danh sách timed effect
            return effects.Where(e => e.RemainingSeconds > 0 && e.EffectType != EffectType.BlockPlay).ToList();
        }

        public async Task<int> GetUtilityRemainAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            return await _cache.GetUtilityRemain(userId);
        }

        public async Task ConsumeUtilityAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            var remain = await _cache.GetUtilityRemain(userId);
            if (remain <= 0)
                throw new ForbiddenException("No utility left");

            //DecrementUtility dùng để trừ đi 1 lần sử dụng hiệu ứng Utility
            await _cache.DecrementUtility(userId);
        }

        public async Task SetUtilityAsync(Guid userId, int count)
        {
            if (count <= 0)
                throw new ValidationException("Utility count must be greater than 0");

            await _cache.SetUtilityRemain(userId, count);
        }

        public async Task UpdateTimedEffectsOnStartTurnAsync(Guid userId)
        {
            Console.WriteLine($"[LOG][EffectService] UpdateTimedEffectsOnStartTurnAsync called for userId={userId}");
            var effects = await _cache.GetTimedEffectsAsync(userId);
            Console.WriteLine($"[LOG][EffectService] Timed effects count: {effects.Count}");
            var now = DateTime.UtcNow;
            foreach (var effect in effects)
            {
                if (effect.LastActiveAt.HasValue)
                {
                    var elapsed = (int)(now - effect.LastActiveAt.Value).TotalSeconds;
                    Console.WriteLine($"[LOG][StartTurn] Effect {effect.EffectType} elapsed: {elapsed}, before: {effect.RemainingSeconds}");
                    effect.RemainingSeconds = Math.Max(0, effect.RemainingSeconds - elapsed);
                    Console.WriteLine($"[LOG][StartTurn] Effect {effect.EffectType} after: {effect.RemainingSeconds}");
                }
                effect.LastActiveAt = now;
            }
            await _cache.SaveTimedEffectsAsync(userId, effects.Where(e => e.RemainingSeconds > 0).ToList());
        }

        public async Task UpdateTimedEffectsOnEndTurnAsync(Guid userId, int playDurationSeconds)
        {
            var effects = await _cache.GetTimedEffectsAsync(userId);
            Console.WriteLine($"[LOG][EndTurn] playDurationSeconds: {playDurationSeconds}");
            foreach (var effect in effects)
            {
                if (effect.LastActiveAt.HasValue)
                {
                    Console.WriteLine($"[LOG][EndTurn] Effect {effect.EffectType} before: {effect.RemainingSeconds}");
                    effect.RemainingSeconds = Math.Max(0, effect.RemainingSeconds - playDurationSeconds);
                    Console.WriteLine($"[LOG][EndTurn] Effect {effect.EffectType} after: {effect.RemainingSeconds}");
                }
                effect.LastActiveAt = null;
            }
            await _cache.SaveTimedEffectsAsync(userId, effects.Where(e => e.RemainingSeconds > 0).ToList());
        }

        public async Task AddOrUpdateTimedEffectAsync(Guid userId, EffectType effectType, int secondsToAdd)
        {
            // BlockPlay: cộng dồn TTL Redis
            if (effectType == EffectType.BlockPlay)
            {
                var ttl = await _cache.GetEffectRemainSeconds(userId, effectType.ToString()) ?? 0;
                await _cache.SetEffect(userId, effectType.ToString(), ttl + secondsToAdd);
                return;
            }

            var effects = await _cache.GetTimedEffectsAsync(userId);
            var effect = effects.FirstOrDefault(e => e.EffectType == effectType);
            if (effect != null)
            {
                effect.RemainingSeconds += secondsToAdd;
            }
            else
            {
                effects.Add(new TimedEffectStateDto
                {
                    EffectType = effectType,
                    RemainingSeconds = secondsToAdd,
                    LastActiveAt = null
                });
            }
            await _cache.SaveTimedEffectsAsync(userId, effects);
        }
    }
}
