using Application.Dto;
using FluentValidation;

public class CaseProceedingRecordCreateDtoValidator : AbstractValidator<CaseProceedingRecordCreateDto>
{
    public CaseProceedingRecordCreateDtoValidator()
    {
        RuleFor(p => p.CaseId)
            .NotEmpty().WithMessage("Case ID is required. Each proceeding must be tied to a case.");

        RuleFor(p => p.JudgeId)
            .NotEmpty()
            .WithMessage("Invalid Judge reference.");

        RuleFor(p => p.CourtSessionId)
            .NotEmpty()
            .WithMessage("Invalid Court Session reference.");

        RuleFor(p => p.ProceedingDate)
            .NotEmpty().WithMessage("Proceeding date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Proceeding date cannot be in the future.");

        RuleFor(p => p.Description)
            .NotEmpty().WithMessage("Proceeding description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(p => p.Outcome)
            .MaximumLength(2000)
            .When(p => !string.IsNullOrWhiteSpace(p.Outcome))
            .WithMessage("Outcome must not exceed 2000 characters.");
    }
}