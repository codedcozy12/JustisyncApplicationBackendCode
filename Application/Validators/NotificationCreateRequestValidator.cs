using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class NotificationCreateRequestValidator : AbstractValidator<NotificationCreateRequestDto>
    {
        public NotificationCreateRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(1000).WithMessage("Message must not exceed 1000 characters.");
            RuleFor(x => x.ToId)
                .NotEmpty().WithMessage("Recipient ID is required.");
        }
    }
}