using System;
using System.Threading.Tasks;
using FidoDino.Application.DTOs.Game;
using FidoDino.Application.Services;

namespace FidoDino.Application.Interface
{
    public interface IPlayTurnService
    {
        Task<PlayTurnResultDto> PlayTurnAsync(Guid userId, Guid sessionId);
    }
}
