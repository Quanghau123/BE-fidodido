using FidoDino.Domain.Entities.System;
using FidoDino.Domain.Interfaces.System;
using FidoDino.Application.Interfaces;
using FidoDino.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using FidoDino.Domain.Enums.Game;
using FidoDino.Infrastructure.Redis;

namespace FidoDino.Application.Services
{
    public class SystemStatusAppService : ISystemStatusAppService
    {
        private readonly ISystemStatusRepository _repo;
        private readonly SystemStatusRedisLoader _systemStatusRedisLoader;
        public SystemStatusAppService(ISystemStatusRepository repo, SystemStatusRedisLoader systemStatusRedisLoader)
        {
            _repo = repo;
            _systemStatusRedisLoader = systemStatusRedisLoader;
        }
        public async Task<SystemStatus?> GetCurrentStatusAsync()
        {
            var status = await _repo.GetCurrentStatusAsync();
            if (status == null)
                throw new NotFoundException("No system status found.");
            return status;
        }
        public async Task UpdateStatusAsync(SystemStatus status)
        {
            if (status == null)
                throw new ArgumentException("Status is required");
            if (!Enum.IsDefined(typeof(SystemStatusType), status.StatusCode))
                throw new ValidationException("StatusCode is invalid");
            await _repo.UpdateStatusAsync(status);
            await _systemStatusRedisLoader.LoadSystemStatusToRedisAsync();
        }
    }
}