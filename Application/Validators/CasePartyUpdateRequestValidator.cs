using Application.Dto;
using FluentValidation;

public class CasePartyUpdateDtoValidator : AbstractValidator<CasePartyUpdateDto>
{
    public CasePartyUpdateDtoValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Party ID is required for updates.");

        RuleFor(p => p.FullName)
            .MaximumLength(150).When(p => !string.IsNullOrWhiteSpace(p.FullName))
            .WithMessage("Full name must not exceed 150 characters.");

        RuleFor(p => p.Address)
            .MaximumLength(300).When(p => !string.IsNullOrWhiteSpace(p.Address))
            .WithMessage("Address must not exceed 300 characters.");

        RuleFor(p => p.Contact)
            .Matches(@"^[0-9+\-()\s]+$")
            .When(p => !string.IsNullOrWhiteSpace(p.Contact))
            .WithMessage("Invalid contact format. Use digits, spaces, or +, -, ().");

        RuleFor(p => p.Role)
            .IsInEnum().When(p => p.Role.HasValue)
            .WithMessage("Invalid party role.");

        RuleFor(p => p.Email)
            .EmailAddress()
            .When(p => !string.IsNullOrWhiteSpace(p.Email))
            .WithMessage("Invalid email format.");
    }
}