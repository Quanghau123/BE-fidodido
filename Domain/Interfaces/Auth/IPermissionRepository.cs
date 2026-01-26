namespace FidoDino.Domain.Interfaces.Auth
{
    public interface IPermissionRepository
    {
        Task<List<string>> GetPermissionsByUserAsync(Guid userId);
    }
}
