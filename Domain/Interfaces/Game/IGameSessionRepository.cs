using FidoDino.Domain.Entities.Game;

namespace FidoDino.Domain.Interfaces.Game
{
    public interface IGameSessionRepository
    {
        Task<GameSession?> GetByIdAsync(Guid sessionId);
        Task<IEnumerable<GameSession>> GetActiveSessionsAsync();
        Task AddAsync(GameSession session);
        Task UpdateAsync(GameSession session);
        Task<IEnumerable<GameSession>> GetByUserIdAsync(Guid userId);
        Task<GameSession?> GetActiveSessionByUserIdAsync(Guid userId);
    }
}