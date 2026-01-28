using FidoDino.Application.DTOs.Auth;

namespace FidoDino.Application.Interfaces
{
    public interface IOAuthClient
    {
        string ProviderName { get; }
        Task<OAuthUserInfoDto> GetUserInfoAsync(string code);
    }
}
