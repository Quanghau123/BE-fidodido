using FidoDino.Application.DTOs.Auth;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Entities;
using FidoDino.Domain.Entities.Auth;
using FidoDino.Domain.Enums.User;
using FidoDino.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace FidoDino.Application.Services
{
    public class OAuthService : IOAuthService
    {
        private readonly FidoDinoDbContext _context;
        private readonly IAuthTokenService _authTokenService;
        private readonly Dictionary<string, IOAuthClient> _clients;

        public OAuthService(
            FidoDinoDbContext context,
            IAuthTokenService authTokenService,
            IEnumerable<IOAuthClient> clients)
        {
            _context = context;
            _authTokenService = authTokenService;
            _clients = clients.ToDictionary(c => c.ProviderName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Đăng nhập bằng OAuth, lấy thông tin người dùng từ provider và xử lý đăng nhập hoặc đăng ký mới.
        /// </summary>
        public async Task<LoginResponseDto> LoginAsync(string provider, string code)
        {
            if (!_clients.TryGetValue(provider, out var client))
                throw new Exception("Provider not supported");

            var userInfo = await client.GetUserInfoAsync(code);
            return await HandleOAuthUserAsync(userInfo);
        }

        /// <summary>
        /// Xử lý thông tin người dùng từ OAuth: kiểm tra tồn tại, tạo mới nếu chưa có, và cấp token đăng nhập.
        /// </summary>
        private async Task<LoginResponseDto> HandleOAuthUserAsync(OAuthUserInfoDto userInfo)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.AuthProvider == userInfo.Provider &&
                u.ProviderUserId == userInfo.ProviderUserId);

            if (user == null)
            {
                user = new User
                {
                    Email = userInfo.Email,
                    UserName = userInfo.Name,
                    AuthProvider = userInfo.Provider,
                    ProviderUserId = userInfo.ProviderUserId,
                    IsActive = true,
                    UserRole = UserRole.Player
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return await _authTokenService.IssueTokenAsync(user);
        }
    }
}
