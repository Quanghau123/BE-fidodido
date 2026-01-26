using FidoDino.Domain.Entities.System;
using FidoDino.Domain.Interfaces.System;
using FidoDino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FidoDino.Persistence.Repositories.System
{
    public class SystemStatusRepository : ISystemStatusRepository
    {
        private readonly FidoDinoDbContext _context;

        public SystemStatusRepository(FidoDinoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy trạng thái hệ thống hiện tại (mới nhất).
        /// </summary>
        public async Task<SystemStatus?> GetCurrentStatusAsync()
        {
            return await _context.SystemStatuses
                .OrderByDescending(s => s.UpdatedAt)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Cập nhật hoặc thêm mới trạng thái hệ thống.
        /// </summary>
        public async Task UpdateStatusAsync(SystemStatus status)
        {
            var existingStatus = await GetCurrentStatusAsync();
            if (existingStatus != null)
            {
                existingStatus.StatusCode = status.StatusCode;
                existingStatus.Message = status.Message;
                existingStatus.UpdatedBy = status.UpdatedBy;
                existingStatus.UpdatedAt = status.UpdatedAt;

                _context.SystemStatuses.Update(existingStatus);
            }
            else
            {
                await _context.SystemStatuses.AddAsync(status);
            }

            await _context.SaveChangesAsync();
        }
    }
}