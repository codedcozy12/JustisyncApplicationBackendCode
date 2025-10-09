using Application.Dto;
using FluentValidation;

public class CaseProceedingRecordUpdateDtoValidator : AbstractValidator<CaseProceedingRecordUpdateDto>
{
    public CaseProceedingRecordUpdateDtoValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Proceeding record ID is required for updates.");

        RuleFor(p => p.JudgeId)
            .NotEmpty()
            .WithMessage("Invalid Judge reference.");

        RuleFor(p => p.CourtSessionId)
            .NotEmpty()
            .WithMessage("Invalid Court Session reference.");

        RuleFor(p => p.ProceedingDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(p => p.ProceedingDate.HasValue)
            .WithMessage("Proceeding date cannot be in the future.");

        RuleFor(p => p.Description)
            .MaximumLength(2000)
            .When(p => !string.IsNullOrWhiteSpace(p.Description))
            .WithMessage("Description must not exceed 2000 characters.");

        RuleFor(p => p.Outcome)
            .MaximumLength(2000)
            .When(p => !string.IsNullOrWhiteSpace(p.Outcome))
            .WithMessage("Outcome must not exceed 2000 characters.");
    }
}