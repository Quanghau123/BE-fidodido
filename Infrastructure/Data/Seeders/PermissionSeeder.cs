using FidoDino.Common.Authorization;
using FidoDino.Domain.Enums.User;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Entities.Auth;

namespace FidoDino.Infrastructure.Data.Seeders
{
    public class PermissionSeeder : ISeeder
    {
        public async Task SeedAsync(FidoDinoDbContext context)
        {
            if (context.Permissions.Any())
                return;

            var permissions = new List<Permission>
            {
                new() { PermissionCode = Permissions.User_View,   Description = "View user" },
                new() { PermissionCode = Permissions.User_Create, Description = "Create user" },
                new() { PermissionCode = Permissions.User_Update, Description = "Update user" },
                new() { PermissionCode = Permissions.User_Delete, Description = "Delete user" },
                new() { PermissionCode = Permissions.User_Export, Description = "Export user" }
            };

            context.Permissions.AddRange(permissions);
            await context.SaveChangesAsync();

            var rolePermissions = new List<RolePermission>
            {
                new() { Role = UserRole.Admin, PermissionId = permissions[0].PermissionId },
                new() { Role = UserRole.Admin, PermissionId = permissions[1].PermissionId },
                new() { Role = UserRole.Admin, PermissionId = permissions[2].PermissionId },
                new() { Role = UserRole.Admin, PermissionId = permissions[3].PermissionId },
                new() { Role = UserRole.Admin, PermissionId = permissions[4].PermissionId },

                new() { Role = UserRole.Staff, PermissionId = permissions[0].PermissionId },
                new() { Role = UserRole.Staff, PermissionId = permissions[2].PermissionId },

                new() { Role = UserRole.Player, PermissionId = permissions[0].PermissionId }
            };

            context.RolePermissions.AddRange(rolePermissions);
            await context.SaveChangesAsync();
        }
    }
}
