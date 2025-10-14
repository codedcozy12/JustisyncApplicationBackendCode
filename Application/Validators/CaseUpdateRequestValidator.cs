using Application.Dto;
using FluentValidation;

public class CaseUpdateDtoValidator : AbstractValidator<CaseUpdateRequestDto>
{
    public CaseUpdateDtoValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Case ID is required for updates.");

        RuleFor(c => c.LawyerId)
            .NotEmpty()
            .When(c => c.LawyerId.HasValue)
            .WithMessage("Invalid lawyer reference.");

        RuleFor(c => c.JudgeId)
            .NotEmpty()
            .When(c => c.JudgeId.HasValue)
            .WithMessage("Invalid judge reference.");

        RuleFor(c => c.OffenseType)
            .IsInEnum()
            .When(c => c.OffenseType.HasValue)
            .WithMessage("Invalid offense type provided.");

        RuleFor(c => c.Description)
            .MaximumLength(1000)
            .When(c => !string.IsNullOrWhiteSpace(c.Description))
            .WithMessage("Description must not exceed 1000 characters.");

        RuleFor(c => c.Status)
            .IsInEnum()
            .When(c => c.Status.HasValue)
            .WithMessage("Invalid case status provided.");
    }
}