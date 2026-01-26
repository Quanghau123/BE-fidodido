namespace FidoDino.Application.DTOs.Auth
{
    public class ResetPasswordRequest
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}