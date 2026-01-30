using Microsoft.AspNetCore.Mvc;

using FidoDino.Application.Interfaces;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth/oauth")]
    public class OAuthController : ControllerBase
    {
        private readonly IOAuthService _oauthService;
        private readonly IConfiguration _configuration;

        public OAuthController(
            IOAuthService oauthService,
            IConfiguration configuration)
        {
            _oauthService = oauthService;
            _configuration = configuration;
        }

        /// <summary>
        /// OAuth login
        /// </summary>
        [HttpGet("{provider}/login")]
        public IActionResult Login(string provider)
        {
            provider = provider.ToLower();

            string url = provider switch
            {
                "google" => BuildGoogleLoginUrl(),
                "facebook" => BuildFacebookLoginUrl(),
                _ => throw new NotSupportedException("Provider not supported")
            };

            return Redirect(url);
        }

        /// <summary>
        /// OAuth callback
        /// </summary>
        [HttpGet("{provider}/callback")]
        public async Task<IActionResult> Callback(
    string provider,
    [FromQuery] string code)
        {
            var result = await _oauthService.LoginAsync(provider, code);

            // Sau khi xử lý xong, redirect về FE kèm accessToken/refreshToken trên URL
            var baseUrl = _configuration["Frontend:BaseUrl"];
            string frontendUrl;
            if (result.AccessToken != null && result.RefreshToken != null)
            {
                frontendUrl = $"{baseUrl}/oauth-callback/{provider}?accessToken={Uri.EscapeDataString(result.AccessToken)}&refreshToken={Uri.EscapeDataString(result.RefreshToken)}";
            }
            else
            {
                frontendUrl = $"{baseUrl}/oauth-callback/{provider}?error={Uri.EscapeDataString("OAuth failed")}";
            }
            return Redirect(frontendUrl);
        }

        private string BuildGoogleLoginUrl()
        {
            var clientId = _configuration["Google:ClientId"];
            var redirectUri = _configuration["Google:RedirectUri"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
            {
                throw new Exception("Google OAuth configuration is missing.");
            }

            return
                "https://accounts.google.com/o/oauth2/v2/auth" +
                "?client_id=" + clientId +
                "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                "&response_type=code" +
                "&scope=openid%20email%20profile" +
                "&access_type=offline" +
                "&prompt=consent";
        }

        private string BuildFacebookLoginUrl()
        {
            var clientId = _configuration["Facebook:ClientId"];
            var redirectUri = _configuration["Facebook:RedirectUri"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
            {
                throw new Exception("Facebook OAuth configuration is missing.");
            }

            return
                "https://www.facebook.com/v18.0/dialog/oauth" +
                "?client_id=" + clientId +
                "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                "&response_type=code" +
                "&scope=public_profile";
        }
    }
}
