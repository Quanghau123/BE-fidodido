using System;
using System.Threading.Tasks;

namespace FidoDino.Application.Interface
{
    public interface IEffectService
    {
        Task<bool> HasEffectAsync(Guid userId, string effectType);
        Task SetEffectAsync(Guid userId, string effectType, int durationSeconds);
        Task RemoveEffectAsync(Guid userId, string effectType);
        Task<int> GetEffectDurationAsync(Guid userId, string effectType);
    }
}
