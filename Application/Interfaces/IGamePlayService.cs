using FidoDino.Application.DTOs.Game;

namespace FidoDino.Application.Interfaces
{
    public interface IGamePlayService
    {
        Task<(IceResultDto ice, int shakeCount)> StartTurnAsync(Guid userId);
        Task<(RewardResultDto reward, int earnedScore)> EndTurnAsync(Guid userId, Guid iceId);
    }
}