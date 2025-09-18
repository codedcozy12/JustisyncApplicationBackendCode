using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class UserForgotPasswordRequestValidator : AbstractValidator<UserForgotPasswordRequestDto>
    {
        public UserForgotPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}