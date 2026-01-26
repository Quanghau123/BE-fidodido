using FidoDino.Domain.Entities.System;

namespace FidoDino.Application.Interfaces
{
    public interface ISystemStatusAppService
    {
        Task<SystemStatus?> GetCurrentStatusAsync();
        Task UpdateStatusAsync(SystemStatus status);
    }
}