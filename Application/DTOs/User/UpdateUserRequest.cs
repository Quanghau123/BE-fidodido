using FidoDino.Domain.Enums.User;

namespace FidoDino.Application.DTOs.User
{
    public class UpdateUserRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
        public UserRole? UserRole { get; set; }
    }
}