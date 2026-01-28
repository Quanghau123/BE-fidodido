using System.ComponentModel.DataAnnotations;
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

        public async Task<bool> HasEffectAsync(Guid userId, EffectType effectType)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (effectType == EffectType.None)
                throw new ArgumentException("EffectType is required");

            return await _cache.HasEffect(userId, effectType.ToString());
        }

        public async Task SetEffectAsync(Guid userId, EffectType effectType, int durationSeconds)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (effectType == EffectType.None)
                throw new ArgumentException("EffectType is required");
            if (durationSeconds <= 0)
                throw new ValidationException("Duration must be greater than 0");

            await _cache.SetEffect(userId, effectType.ToString(), durationSeconds);
        }

        public async Task RemoveEffectAsync(Guid userId, EffectType effectType)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (effectType == EffectType.None)
                throw new ArgumentException("EffectType is required");

            await _cache.RemoveEffect(userId, effectType.ToString());
        }

        public async Task<int> GetEffectDurationAsync(Guid userId, EffectType effectType)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");
            if (effectType == EffectType.None)
                throw new ArgumentException("EffectType is required");

            var remain = await _cache.GetEffectRemainSeconds(userId, effectType.ToString());
            if (remain == null)
                throw new NotFoundException("Effect not found for user");

            return remain.Value;
        }

        public async Task ConsumeUtilityAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            var remain = await _cache.GetUtilityRemain(userId);
            if (remain <= 0)
                throw new ForbiddenException("No utility left");

            await _cache.DecrementUtility(userId);
        }

        public async Task SetUtilityAsync(Guid userId, int count)
        {
            if (count <= 0)
                throw new ValidationException("Utility count must be greater than 0");

            await _cache.SetUtilityRemain(userId, count);
        }
    }
}
