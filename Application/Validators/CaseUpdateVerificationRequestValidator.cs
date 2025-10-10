using Application.Dto;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators
{
    public class CaseUpdateVerificationRequestValidator : AbstractValidator<CaseUpdateVerificationRequestDto>
    {
        public CaseUpdateVerificationRequestValidator()
        {
            RuleFor(x => x.CaseId)
                .NotEmpty().WithMessage("Case ID is required.");

            RuleFor(x => x.ApprovalStatus)
                .IsInEnum().WithMessage("Invalid approval status value.");

            When(x => x.ApprovalStatus == CaseApprovalStatus.Rejected, () =>
            {
                RuleFor(x => x.RejectionReason)
                    .NotEmpty().WithMessage("Rejection reason is required when rejecting a case.")
                    .MaximumLength(700).WithMessage("Rejection reason must not exceed 700 characters.");
            });

            When(x => x.ApprovalStatus == CaseApprovalStatus.Approved, () =>
            {
                RuleFor(x => x.RejectionReason)
                    .Empty().WithMessage("Rejection reason should be empty when approving a case.");
            });
        }
    }
}