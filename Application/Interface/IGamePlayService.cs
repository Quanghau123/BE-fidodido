using FidoDino.Application.DTOs;
using FidoDino.Application.DTOs.Game;
using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Entities.System;

namespace FidoDino.Application.Interfaces
{
    public interface IGamePlayService
    {
        Task<IceResultDto> RandomIceAndApplyEffect(Guid userId);
        Task<RewardResultDto> RandomReward(Guid iceId);
        Task<PlayTurnResultDto> CompletePlayTurn(Guid sessionId, Guid iceId, Guid rewardId, int shakeCount, int earnedScore);
    }
}