using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class RegistrarUpdateRequestValidator : AbstractValidator<RegistrarUpdateRequestDto>
    {
        public RegistrarUpdateRequestValidator()
        {
            RuleFor(x => x.CourtId)
            .NotEmpty().WithMessage("CourtAssignedId is required when updating a Registrar.");
        }
    }
}