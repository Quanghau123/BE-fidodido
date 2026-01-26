using FidoDino.Domain.Entities.Game;

namespace FidoDino.Domain.Interfaces.Game
{
    public interface IEffectRepository
    {
        Task<Effect?> GetByIdAsync(Guid effectId);
        Task<IEnumerable<Effect>> GetAllAsync();
    }
}