using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class CertificateUploadRequestValidator : AbstractValidator<CertificateUploadRequestDto>
    {
        public CertificateUploadRequestValidator() 
        {
            RuleFor(x => x.LawyerId)
            .NotEmpty().WithMessage("LawyerId is required.");

            RuleFor(x => x.Files)
                .NotNull().WithMessage("At least one certificate file is required.")
                .Must(files => files != null && files.Any()).WithMessage("At least one certificate file must be provided.");

            RuleForEach(x => x.Files).ChildRules(file =>
            {
                file.RuleFor(f => f.Length)
                    .GreaterThan(0).WithMessage("Uploaded file cannot be empty.")
                    .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("Each file size must not exceed 10MB.");

                file.RuleFor(f => f.FileName)
                    .NotEmpty().WithMessage("File name is required.")
                    .Must(name => name.EndsWith(".pdf") || name.EndsWith(".jpg") || name.EndsWith(".png"))
                    .WithMessage("Only PDF, JPG, or PNG files are allowed.");
            });

            RuleFor(x => x.CertificateType)
                .NotNull().WithMessage("Certificate type list is required.")
                .Must(types => types != null && types.Any()).WithMessage("At least one certificate type must be provided.");

            RuleFor(x => x)
                .Must(x => x.Files.Count == x.CertificateType.Count)
                .WithMessage("Each uploaded file must have a corresponding certificate type.");
        }
    }
}