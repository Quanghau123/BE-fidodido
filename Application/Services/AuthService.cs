using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

using FidoDino.Application.DTOs.Auth;
using FidoDino.Application.Interfaces;
using FidoDino.Infrastructure.Data;
using FidoDino.Common.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FidoDino.Domain.Entities.Auth;
using FidoDino.Domain.Interfaces.Auth;

namespace FidoDino.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly FidoDinoDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IEmailService _emailService;
        private readonly IAuthTokenService _authTokenService;

        public AuthService(
            FidoDinoDbContext context,
            IJwtTokenService jwtTokenService,
            IPermissionRepository permissionRepository,
            IEmailService emailService,
            IAuthTokenService authTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _permissionRepository = permissionRepository;
            _emailService = emailService;
            _authTokenService = authTokenService;
        }

        /// <summary>
        /// Xử lý đăng nhập người dùng, kiểm tra email, mật khẩu và trạng thái tài khoản.
        /// </summary>
        public async Task<LoginResponseDto> HandleUserLoginAsync(LoginRequest request)
        {
            var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid email or password");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("User is inactive");

            return await _authTokenService.IssueTokenAsync(user);
        }
        /// <summary>
        /// Làm mới access token bằng refresh token, đồng thời thu hồi refresh token cũ và cấp refresh token mới.
        /// </summary>
        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var dbToken = await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Token == refreshToken &&
                    !x.IsRevoked &&
                    x.ExpiresAt > DateTime.UtcNow);

            if (dbToken == null)
                throw new SecurityTokenException("Invalid refresh token");

            var user = dbToken.User;

            if (!user.IsActive)
                throw new UnauthorizedAccessException("User is inactive");

            dbToken.IsRevoked = true;

            var newRefreshToken = CreateRefreshToken(user.UserId);
            await _context.RefreshTokens.AddAsync(newRefreshToken);

            var permissions =
                await _permissionRepository.GetPermissionsByUserAsync(user.UserId);
            var newAccessToken =
                _jwtTokenService.GenerateAccessToken(user, permissions);

            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }
        /// <summary>
        /// Thu hồi (revoke) tất cả refresh token còn hiệu lực của người dùng khi đăng xuất.
        /// </summary>
        public async Task LogoutAsync(Guid userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(x => x.UserId == userId && !x.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
                token.IsRevoked = true;

            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Tạo refresh token mới cho người dùng với thời hạn 7 ngày.
        /// </summary>
        private static RefreshToken CreateRefreshToken(Guid userId)
        {
            return new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
        }

        /// <summary>
        /// Gửi email chứa link đặt lại mật khẩu cho người dùng, kiểm tra chống spam gửi mail.
        /// </summary>
        public async Task SendPasswordResetLinkAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return;

            //chống spam gửi mail
            var recentToken = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.UserId && t.UsedAt == null)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (recentToken != null &&
             recentToken.ExpiresAt > DateTime.UtcNow &&
                recentToken.CreatedAt > DateTime.UtcNow.AddMinutes(-15))
            {
                throw new ConflictException(
                    "Password reset was requested recently. Please check your email.");
            }

            await _context.PasswordResetTokens
                .Where(t => t.UserId == user.UserId && t.UsedAt == null)
                // Cập nhật hàng loạt (bulk update) trực tiếp trên database mà không cần load các entity về bộ nhớ.
                .ExecuteUpdateAsync(t =>
                    t.SetProperty(x => x.UsedAt, DateTime.UtcNow));

            var rawToken = GenerateResetToken();
            var tokenHash = BCrypt.Net.BCrypt.HashPassword(rawToken);

            var resetToken = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.UserId,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };

            await _context.PasswordResetTokens.AddAsync(resetToken);
            await _context.SaveChangesAsync();

            var resetLink =
                $"http://localhost:3000/reset-password" +
                $"?userId={user.UserId}&token={Uri.EscapeDataString(rawToken)}";

            var emailBody = $@"
                <p>Hi {user.UserName},</p>
                <p>Click the link below to reset your password. The link expires in 2 hours.</p>
                <a href='{resetLink}'>Reset Password</a>
            ";

            try
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Password Reset Request",
                    emailBody);
            }
            catch (Exception)
            {
                throw new ConflictException(
                    "Unable to send reset email. Please try again later.");
            }
        }

        /// <summary>
        /// Đặt lại mật khẩu mới cho người dùng sau khi xác thực token hợp lệ.
        /// </summary>
        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            var resetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.UserId == request.UserId &&
                    t.UsedAt == null);

            if (resetToken == null)
                throw new NotFoundException("Reset token not found");

            if (resetToken.ExpiresAt < DateTime.UtcNow)
                throw new ValidationException("Reset token expired");

            if (!BCrypt.Net.BCrypt.Verify(request.Token, resetToken.TokenHash))
                throw new ValidationException("Invalid reset token");

            resetToken.User.Password =
                BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            resetToken.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Sinh chuỗi token ngẫu nhiên dùng cho đặt lại mật khẩu.
        /// </summary>
        private static string GenerateResetToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }
    }
}
