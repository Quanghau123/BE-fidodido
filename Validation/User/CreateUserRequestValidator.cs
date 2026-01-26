using FluentValidation;

using FidoDino.Application.DTOs.User;

namespace FidoDino.Application.Validation.User
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required")
                .MinimumLength(4).WithMessage("UserName must be at least 4 characters")
                .MaximumLength(32).WithMessage("UserName must be at most 32 characters")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("UserName only allows letters, numbers, and underscores");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is invalid");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.UserRole)
                .NotEmpty().WithMessage("UserRole is required")
                .Must(BeValidRole).WithMessage("Invalid user role");
        }

        private bool BeValidRole(string role)
            => Enum.TryParse<FidoDino.Domain.Enums.User.UserRole>(role, true, out _);
    }
}
