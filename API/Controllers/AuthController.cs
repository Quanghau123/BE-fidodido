using Microsoft.AspNetCore.Mvc;

using FidoDino.Application.Interfaces;
using FidoDino.Application.DTOs.Auth;
using FidoDino.Common;

using System.Security.Claims;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// [1.1] Đăng nhập
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest data)
        {
            var result = await _authService.HandleUserLoginAsync(data);
            return Ok(new ApiResponse<LoginResponseDto>(true, "Login successful.", result));
        }

        /// <summary>
        /// [1.2] Refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto data)
        {
            var result = await _authService.RefreshTokenAsync(data.RefreshToken);
            return Ok(new ApiResponse<TokenResponseDto>(true, "Token refreshed successfully.", result));
        }

        /// <summary>
        /// [1.3] Logout
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _authService.LogoutAsync(userId);

            return Ok(new ApiResponse<object>(true, "Logout successful."));
        }

        /// <summary>
        /// [1.4] Quên mật khẩu
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _authService.SendPasswordResetLinkAsync(request.Email);
            return Ok(new ApiResponse<object>(true, "If the email exists, a reset link has been sent."));
        }

        /// <summary>
        /// [1.5] Reset mật khẩu
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _authService.ResetPasswordAsync(request);
            return Ok(new ApiResponse<object>(true, "Password has been reset successfully."));
        }
    }
}
