using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class CertificateCreateRequestValidator : AbstractValidator<CertificateCreateRequestDto>
    {
        public CertificateCreateRequestValidator()
        {
            RuleFor(x => x.CertificateType)
            .NotEmpty().WithMessage("Certificate type is required.")
            .MaximumLength(50).WithMessage("Certificate type must not exceed 50 characters.");
            
            RuleFor(x => x.File)
            .NotNull().WithMessage("Certificate file is required.")
            .Must(file => file.Length > 0).WithMessage("Uploaded file cannot be empty.")
            .Must(file => new[] { ".pdf", ".jpg", ".jpeg", ".png" }
            .Contains(Path.GetExtension(file.FileName).ToLower()))
            .WithMessage("Only PDF or image files are allowed.")
            .Must(file => file.Length <= 10 * 1024 * 1024)
            .WithMessage("File size cannot exceed 5MB.");

            RuleFor(x => x.LawyerId)
                .NotEmpty().WithMessage("LawyerId is required.");
        }
    }
}