using System;
using System.Threading.Tasks;
using FidoDino.Application.DTOs.Game;
using FidoDino.Domain.Enums.Game;

namespace FidoDino.Application.Interfaces
{
    public interface IEffectService
    {
        Task<List<ActiveEffectDto>> GetAllActiveEffectsAsync(Guid userId);
        Task<bool> HasEffectAsync(Guid userId, EffectType effectType);
        Task SetEffectAsync(Guid userId, EffectType effectType, int durationSeconds);
        Task RemoveEffectAsync(Guid userId, EffectType effectType);
        Task<int> GetEffectDurationAsync(Guid userId, EffectType effectType);
        Task ConsumeUtilityAsync(Guid userId);
        Task SetUtilityAsync(Guid userId, int count);
        Task<int> GetUtilityRemainAsync(Guid userId);
        Task UpdateTimedEffectsOnStartTurnAsync(Guid userId);
        Task UpdateTimedEffectsOnEndTurnAsync(Guid userId, int playDurationSeconds);
        Task AddOrUpdateTimedEffectAsync(Guid userId, EffectType effectType, int secondsToAdd);
        Task<List<TimedEffectStateDto>> GetActiveTimedEffectsAsync(Guid userId);
        Task<StartTurnEffectResultDto> ApplyStartTurnEffectsAsync(Guid userId, int baseShakeCount);
        Task<EndTurnEffectResultDto> ApplyEndTurnEffectsAsync(int baseScore, bool hadDoubleScore);
        Task<RewardEffectResultDto> ApplyRewardEffectAsync(Guid userId, EffectType effectType);
    }
}
