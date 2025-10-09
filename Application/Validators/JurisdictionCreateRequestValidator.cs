using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class JurisdictionCreateRequestValidator : AbstractValidator<JurisdictionCreateRequestDto>
    {
        public JurisdictionCreateRequestValidator()
        {
            RuleFor(j => j.SubjectMatter)
                .NotEmpty().WithMessage("Subject matter is required.")
                .MinimumLength(3).WithMessage("Subject matter must be at least 3 characters long.")
                .MaximumLength(500).WithMessage("Subject matter must not exceed 500 characters.");

            RuleFor(j => j.MonetaryLimit)
                .GreaterThan(0).When(j => j.MonetaryLimit.HasValue)
                .WithMessage("Monetary limit must be greater than zero if provided.");
        }
    }
}