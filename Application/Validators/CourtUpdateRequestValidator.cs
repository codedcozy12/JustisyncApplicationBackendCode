using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class CourtUpdateRequestValidator : AbstractValidator<CourtUpdateRequestDto>
    {
        public CourtUpdateRequestValidator()
        {
            RuleFor(x => x.ContactEmail)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));

            RuleFor(x => x.ContactPhone)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPhone));

            //RuleFor(x => x.Jurisdiction)
            //.IsInEnum()
            //.WithMessage("Invalid jurisdiction value.");
        }
    }
}