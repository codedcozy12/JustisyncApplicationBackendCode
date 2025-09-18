using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class UserResetPasswordRequestValidator : AbstractValidator<UserResetPasswordRequestDto>
    {
        public UserResetPasswordRequestValidator()
        {
             
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.OldPassword)
                 .NotEmpty();
        }
    }
}