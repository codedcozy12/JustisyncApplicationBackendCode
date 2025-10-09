using Application.Dto;
using Domain.Entities;
using FluentValidation;

namespace Application.Validators
{
    public class CaseCreateRequestValidator : AbstractValidator<CaseCreateRequestDto>
    {
        public CaseCreateRequestValidator() 
        {
            RuleFor(c => c.CaseNumber)
            .NotEmpty().WithMessage("Case number is required.")
            .MaximumLength(50).WithMessage("Case number must not exceed 50 characters.");

        RuleFor(c => c.ClientId)
            .NotEmpty().WithMessage("Client ID is required. A case must be filed by a client.");

        RuleFor(c => c.LawyerId)
            .NotEmpty().WithMessage("Lawyer ID is required. Each case must be assigned to a lawyer.");

        //RuleFor(c => c.CourtId)
        //    .NotEmpty().WithMessage("Court ID is required. Please assign the case to a valid court.");

        //RuleFor(c => c.JudgeId)
        //    .NotEmpty().WithMessage("Judge ID is required. A case must have an overseeing judge.");

        RuleFor(c => c.OffenseType)
            .IsInEnum().WithMessage("Invalid offense type specified.");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Case description is required.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(c => c.Status)
            .IsInEnum().WithMessage("Invalid case status value.");

        RuleForEach(c => c.Parties)
            .SetValidator(new CasePartyCreateDtoValidator()!)
            .When(c => c.Parties != null && c.Parties.Any(), ApplyConditionTo.CurrentValidator);
        }
    }
}