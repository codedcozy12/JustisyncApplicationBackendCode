using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class LawyerCreateRequestValidator : AbstractValidator<LawyerCreateRequestDto>
    {
        public LawyerCreateRequestValidator()
        {
            RuleFor(x => x.UniversityName)
                .NotEmpty().WithMessage("University name is required.")
                .MaximumLength(100).WithMessage("University name must not exceed 100 characters.");

            RuleFor(x => x.BarMembershipNumber)
                .NotEmpty().WithMessage("Bar membership number is required.")
                .Matches("^[A-Za-z0-9-]+$").WithMessage("Bar membership number must only contain letters, numbers, or dashes.");

            RuleFor(x => x.LawyerSpecialization)
                .IsInEnum().WithMessage("Invalid lawyer specialization.");

            RuleFor(x => x.CertificateTypes)
                .NotEmpty().WithMessage("At least one certificate is required.");

            RuleForEach(x => x.CertificateFiles).ChildRules(file =>
            {
                file.RuleFor(f => f.Length)
                    .GreaterThan(0).WithMessage("Uploaded certificate file cannot be empty.");

                file.RuleFor(f => f.ContentType)
                    .Must(ct => ct == "application/pdf" || ct.StartsWith("image/"))
                    .WithMessage("Certificate must be a PDF or an image.");
            });
        }
    }
}