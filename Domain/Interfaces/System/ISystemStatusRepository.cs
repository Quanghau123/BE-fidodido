using FidoDino.Domain.Entities.System;

namespace FidoDino.Domain.Interfaces.System
{
    public interface ISystemStatusRepository
    {
        Task<SystemStatus?> GetCurrentStatusAsync();
        Task UpdateStatusAsync(SystemStatus status);
    }
}