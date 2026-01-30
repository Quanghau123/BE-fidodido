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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest data)
        {
            var result = await _authService.HandleUserLoginAsync(data);
            return Ok(new ApiResponse<LoginResponseDto>(true, "Login successful.", result));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto data)
        {
            var result = await _authService.RefreshTokenAsync(data.RefreshToken);
            return Ok(new ApiResponse<TokenResponseDto>(true, "Token refreshed successfully.", result));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _authService.LogoutAsync(userId);

            return Ok(new ApiResponse<object>(true, "Logout successful."));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _authService.SendPasswordResetLinkAsync(request.Email);
            return Ok(new ApiResponse<object>(true, "If the email exists, a reset link has been sent."));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _authService.ResetPasswordAsync(request);
            return Ok(new ApiResponse<object>(true, "Password has been reset successfully."));
        }
    }
}
