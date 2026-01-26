namespace FidoDino.Domain.Entities.Auth
{
    public class Permission
    {
        public Guid PermissionId { get; set; }
        public string PermissionCode { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}