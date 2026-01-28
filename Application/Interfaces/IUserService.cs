using FidoDino.Application.DTOs.User;

namespace FidoDino.Application.Interfaces
{
    public interface IUserService
    {
        Task<object> CreateNewUserAsync(CreateUserRequest data);
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<object> HandleUpdateUserAsync(Guid userId, UpdateUserRequest data);
        Task<object> HandleDeleteUserAsync(Guid userId);
        Task<Stream> ExportUsersToCsvAsync(ExportUserRequest request, CancellationToken ct);
    }
}