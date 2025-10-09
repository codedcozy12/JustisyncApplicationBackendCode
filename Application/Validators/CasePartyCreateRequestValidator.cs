using Application.Dto;
using FluentValidation;

public class CasePartyCreateDtoValidator : AbstractValidator<CasePartyCreateDto>
{
    public CasePartyCreateDtoValidator()
    {
        RuleFor(p => p.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(150).WithMessage("Full name must not exceed 150 characters.");

        RuleFor(p => p.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(300).WithMessage("Address must not exceed 300 characters.");

        RuleFor(p => p.Contact)
            .NotEmpty().WithMessage("Contact number is required.")
            .Matches(@"^[0-9+\-()\s]+$").WithMessage("Invalid contact format. Use digits, spaces, or +, -, ().");

        RuleFor(p => p.Role)
            .IsInEnum().WithMessage("Invalid party role.");

        RuleFor(p => p.Email)
            .EmailAddress()
            .When(p => !string.IsNullOrWhiteSpace(p.Email))
            .WithMessage("Invalid email address format.");
    }
}
