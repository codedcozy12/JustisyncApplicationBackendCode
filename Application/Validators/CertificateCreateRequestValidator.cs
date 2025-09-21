using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class CertificateCreateRequestDtoValidator : AbstractValidator<CertificateCreateRequestDto>
    {
        public CertificateCreateRequestDtoValidator()
        {
            RuleFor(x => x.LawyerId)
                .NotEmpty().WithMessage("LawyerId is required.");

            RuleFor(x => x.CertificateType)
                .NotEmpty().WithMessage("Certificate type is required.");

            RuleFor(x => x.File)
                .NotNull().WithMessage("Certificate file is required.")
                .Must(f => f.Length > 0).WithMessage("Uploaded file cannot be empty.")
                .Must(f => f.Length <= 10 * 1024 * 1024).WithMessage("File size must not exceed 10MB.")
                .Must(f => f.FileName.EndsWith(".pdf") || f.FileName.EndsWith(".jpg") || f.FileName.EndsWith(".png"))
                .WithMessage("Only PDF, JPG, or PNG files are allowed.");
        }
    }
}