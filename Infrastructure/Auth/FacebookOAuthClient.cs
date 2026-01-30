using System.Text.Json;

using FidoDino.Application.Interfaces;
using FidoDino.Domain.Enums.User;
using FidoDino.Application.DTOs.Auth;

using Microsoft.Extensions.Options;

namespace FidoDino.Infrastructure.Auth
{
    public class FacebookOAuthClient : IOAuthClient
    {
        public string ProviderName => "facebook";
        private readonly HttpClient _httpClient;
        private readonly FacebookOAuthOptions _options;

        public FacebookOAuthClient(HttpClient httpClient, IOptions<FacebookOAuthOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<OAuthUserInfoDto> GetUserInfoAsync(string code)
        {
            // Gọi API Facebook để lấy access token
            var tokenResponse = await _httpClient.GetAsync(
                $"https://graph.facebook.com/v18.0/oauth/access_token" +
                $"?client_id={_options.ClientId}" +
                $"&redirect_uri={_options.RedirectUri}" +
                $"&client_secret={_options.ClientSecret}" +
                $"&code={code}"
            );

            tokenResponse.EnsureSuccessStatusCode();

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            using var tokenDoc = JsonDocument.Parse(tokenJson);
            var accessToken = tokenDoc.RootElement.GetProperty("access_token").GetString();

            // Gọi API Facebook để lấy thông tin người dùng
            var userResponse = await _httpClient.GetAsync(
                $"https://graph.facebook.com/me" +
                $"?fields=id,name,email" +
                $"&access_token={accessToken}"
            );

            userResponse.EnsureSuccessStatusCode();

            var userJson = await userResponse.Content.ReadAsStringAsync();
            using var userDoc = JsonDocument.Parse(userJson);

            return new OAuthUserInfoDto
            {
                Provider = AuthProvider.Facebook,
                ProviderUserId = userDoc.RootElement.GetProperty("id").GetString()!,
                Email = userDoc.RootElement.TryGetProperty("email", out var email)
                    ? email.GetString() ?? ""
                    : "",
                Name = userDoc.RootElement.GetProperty("name").GetString() ?? ""
            };
        }
    }
}
