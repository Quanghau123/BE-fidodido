using FidoDino.Application.DTOs.Auth;

namespace FidoDino.Application.Interfaces
{
    public interface IOAuthService
    {
        Task<LoginResponseDto> LoginAsync(string provider, string code);
    }
}
