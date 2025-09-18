using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class LawyerUpdateRequestValidator : AbstractValidator<LawyerUpdateRequestDto>
    {
        public LawyerUpdateRequestValidator()
        {
            RuleFor(x => x.UniversityName)
            .NotEmpty().WithMessage("University name is required.")
            .MaximumLength(100).WithMessage("University name must not exceed 100 characters.");

            RuleFor(x => x.BarMembershipNumber)
                .NotEmpty().WithMessage("Bar membership number is required.")
                .Matches("^[A-Za-z0-9-]+$").WithMessage("Bar membership number must only contain letters, numbers, or dashes.");

            RuleFor(x => x.LawyerSpecialization)
                .IsInEnum().WithMessage("Invalid lawyer specialization.");

            RuleForEach(x => x.Certificates)
                .SetValidator(new CertificateUpdateRequestValidator())
                .WithMessage("One or more certificates are invalid.");

            RuleFor(x => x.Certificates)
                .NotEmpty().WithMessage("At least one certificate is required.");
        }
    }
}