using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class CertificateUpdateRequestValidator : AbstractValidator<CertificateUpdateRequestDto>
    {
        public CertificateUpdateRequestValidator()
        {
            RuleFor(x => x.IsVerified)
            .NotNull().WithMessage("Verification status must be provided.");

            RuleFor(x => x.VerifiedByUserId)
                .NotEmpty().When(x => x.IsVerified)
                .WithMessage("VerifiedByUserId is required when marking a certificate as verified.");
        }
    }
}