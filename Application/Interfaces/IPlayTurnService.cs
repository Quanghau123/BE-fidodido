using FidoDino.Application.DTOs.Game;

namespace FidoDino.Application.Interfaces
{
    public interface IPlayTurnService
    {
        Task<StartTurnResultDto> StartTurnAsync(Guid userId, Guid sessionId);
        Task<PlayTurnResultDto> EndTurnAsync(Guid userId);
    }
}
