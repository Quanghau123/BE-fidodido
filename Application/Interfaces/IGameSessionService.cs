using FidoDino.Application.DTOs.Game;
using FidoDino.Domain.Entities.Game;

namespace FidoDino.Application.Interfaces
{
    public interface IGameSessionService
    {
        Task<GameSession?> GetActiveSessionAsync(Guid userId);
        Task<GameSessionDto> StartSessionAsync(Guid userId);
        Task EndSessionAsync(Guid sessionId);
    }
}