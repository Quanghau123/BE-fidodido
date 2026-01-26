using FidoDino.Application.DTOs.Auth;
using FidoDino.Application.Interfaces;

using FidoDino.Domain.Entities.Auth;
using FidoDino.Domain.Interfaces.Auth;
using FidoDino.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace FidoDino.Application.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly FidoDinoDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPermissionRepository _permissionRepository;

        public AuthTokenService(
                FidoDinoDbContext context,
                IJwtTokenService jwtTokenService,
                IPermissionRepository permissionRepository)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _permissionRepository = permissionRepository;
        }

        /// <summary>
        /// Thu hồi các refresh token cũ, cấp mới access token và refresh token cho người dùng khi đăng nhập.
        /// </summary>
        public async Task<LoginResponseDto> IssueTokenAsync(User user)
        {
            var oldTokens = await _context.RefreshTokens
                .Where(x => x.UserId == user.UserId && !x.IsRevoked)
                .ToListAsync();

            foreach (var token in oldTokens)
                token.IsRevoked = true;

            var permissions =
                await _permissionRepository.GetPermissionsByUserAsync(user.UserId);

            var accessToken =
                _jwtTokenService.GenerateAccessToken(user, permissions);

            var refreshToken = new RefreshToken
            {
                UserId = user.UserId,
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
