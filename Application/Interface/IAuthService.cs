using FidoDino.Application.DTOs.Auth;

namespace FidoDino.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> HandleUserLoginAsync(LoginRequest request);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(Guid userId);
        Task SendPasswordResetLinkAsync(string email);
        Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}