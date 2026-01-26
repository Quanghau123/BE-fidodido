using FidoDino.Domain.Entities.Game;

namespace FidoDino.Domain.Interfaces.Game
{
    public interface IActiveEffectRepository
    {
        Task<IEnumerable<ActiveEffect>> GetBySessionIdAsync(Guid sessionId);
        Task AddAsync(ActiveEffect activeEffect);
        Task RemoveAsync(Guid activeEffectId);
    }
}