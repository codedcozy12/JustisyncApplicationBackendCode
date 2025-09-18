using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class JudgeUpdateRequestValidator : AbstractValidator<JudgeUpdateRequestDto>
    {
        public JudgeUpdateRequestValidator()
        {
            RuleFor(x => x.CourtAssignedId)
            .NotEmpty().WithMessage("CourtAssignedId is required.");

            RuleFor(x => x.AppointmentDate)
                .Must(date => date == null || date >= DateTime.UtcNow.Date)
                .WithMessage("Appointment date cannot be in the past.");
        }
    }
}