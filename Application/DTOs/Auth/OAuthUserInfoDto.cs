using FidoDino.Domain.Enums.User;

namespace FidoDino.Application.DTOs.Auth
{
    public class OAuthUserInfoDto
    {
        public string ProviderUserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public AuthProvider Provider { get; set; }
    }
}
