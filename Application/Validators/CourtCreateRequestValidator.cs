using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class CourtCreateRequestValidator : AbstractValidator<CourtCreateRequestDto>
    {
        public CourtCreateRequestValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Court name is required.")
            .MaximumLength(200).WithMessage("Court name must not exceed 200 characters.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Court type is required.")
                .MaximumLength(100).WithMessage("Court type must not exceed 100 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(300).WithMessage("Address must not exceed 300 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100);

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100);

            RuleFor(x => x.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("Contact phone is required.")
                .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Invalid phone number format. Use digits only and optional + prefix.");

            RuleFor(x => x.Jurisdiction)
                .IsInEnum().WithMessage("Invalid jurisdiction value.");
        }
    }
}