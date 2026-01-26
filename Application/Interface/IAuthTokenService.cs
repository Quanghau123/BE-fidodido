using FidoDino.Application.DTOs.Auth;
using FidoDino.Domain.Entities.Auth;

namespace FidoDino.Application.Interfaces
{
    public interface IAuthTokenService
    {
        Task<LoginResponseDto> IssueTokenAsync(User user);
    }
}
