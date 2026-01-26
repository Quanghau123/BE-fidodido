using FidoDino.Infrastructure.Data;
using FidoDino.Domain.Interfaces.Auth;

using Microsoft.EntityFrameworkCore;

namespace FidoDino.Persistence.Repositories.Auth
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly FidoDinoDbContext _context;

        public PermissionRepository(FidoDinoDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetPermissionsByUserAsync(Guid userId)
        {
            var role = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.UserRole)
                .FirstAsync();

            return await _context.RolePermissions
                .Where(rp => rp.Role == role)
                .Select(rp => rp.Permission.PermissionCode)
                .ToListAsync();
        }
    }
}
