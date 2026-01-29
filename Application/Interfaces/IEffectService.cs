using System;
using System.Threading.Tasks;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Interfaces
{
    public interface IEffectService
    {
        Task<bool> HasEffectAsync(Guid userId, EffectType effectType);
        Task SetEffectAsync(Guid userId, EffectType effectType, int durationSeconds);
        Task RemoveEffectAsync(Guid userId, EffectType effectType);
        Task<int> GetEffectDurationAsync(Guid userId, EffectType effectType);
        Task ConsumeUtilityAsync(Guid userId);
        Task SetUtilityAsync(Guid userId, int count);
        Task<int> GetUtilityRemainAsync(Guid userId);
    }
}
