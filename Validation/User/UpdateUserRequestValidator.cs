using FluentValidation;

using FidoDino.Application.DTOs.User;

namespace FidoDino.Application.Validation.User
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.UserName)
                .MinimumLength(4).When(x => !string.IsNullOrWhiteSpace(x.UserName))
                .MaximumLength(32).When(x => !string.IsNullOrWhiteSpace(x.UserName))
                .Matches("^[a-zA-Z0-9_]+$").When(x => !string.IsNullOrWhiteSpace(x.UserName));

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.UserRole)
                .Must(role => !role.HasValue || Enum.IsDefined(typeof(FidoDino.Domain.Enums.User.UserRole), role.Value))
                .WithMessage("Invalid user role");
        }
    }
}
