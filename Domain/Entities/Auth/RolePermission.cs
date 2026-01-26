using FidoDino.Domain.Enums.User;

namespace FidoDino.Domain.Entities.Auth
{
    public class RolePermission
    {
        public Guid RolePermissionId { get; set; }
        public UserRole Role { get; set; }
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}