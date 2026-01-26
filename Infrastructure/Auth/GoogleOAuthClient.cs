using System.Net.Http.Headers;
using System.Text.Json;

using FidoDino.Domain.Enums.User;
using FidoDino.Application.Interfaces;
using FidoDino.Application.DTOs.Auth;

using Microsoft.Extensions.Options;

namespace FidoDino.Infrastructure.Auth
{
    public class GoogleOAuthClient : IOAuthClient
    {
        public string ProviderName => "google";
        private readonly HttpClient _httpClient;
        private readonly GoogleOAuthOptions _options;

        public GoogleOAuthClient(HttpClient httpClient, IOptions<GoogleOAuthOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<OAuthUserInfoDto> GetUserInfoAsync(string code)
        {
            var tokenResponse = await _httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = _options.ClientId,
                    ["client_secret"] = _options.ClientSecret,
                    ["code"] = code,
                    ["grant_type"] = "authorization_code",
                    ["redirect_uri"] = _options.RedirectUri
                })
            );

            tokenResponse.EnsureSuccessStatusCode();

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            using var tokenDoc = JsonDocument.Parse(tokenJson);
            var accessToken = tokenDoc.RootElement.GetProperty("access_token").GetString();

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://www.googleapis.com/oauth2/v3/userinfo");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var userResponse = await _httpClient.SendAsync(request);
            userResponse.EnsureSuccessStatusCode();

            var userJson = await userResponse.Content.ReadAsStringAsync();
            using var userDoc = JsonDocument.Parse(userJson);

            return new OAuthUserInfoDto
            {
                Provider = AuthProvider.Google,
                ProviderUserId = userDoc.RootElement.GetProperty("sub").GetString()!,
                Email = userDoc.RootElement.GetProperty("email").GetString() ?? "",
                Name = userDoc.RootElement.GetProperty("name").GetString() ?? ""
            };
        }
    }
}
