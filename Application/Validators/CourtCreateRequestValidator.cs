using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class CourtCreateRequestValidator : AbstractValidator<CourtCreateRequestDto>
    {
        public CourtCreateRequestValidator()
        {
            RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Court name is required.")
            .MaximumLength(200).WithMessage("Court name cannot exceed 200 characters.");

            RuleFor(c => c.Type)
                .NotEmpty().WithMessage("Court type is required.")
                .MaximumLength(100).WithMessage("Court type cannot exceed 100 characters.");

            RuleFor(c => c.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(300).WithMessage("Address cannot exceed 300 characters.");

            RuleFor(c => c.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

            RuleFor(c => c.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

            RuleFor(c => c.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required.")
                .EmailAddress().WithMessage("Contact email must be a valid email address.")
                .MaximumLength(150).WithMessage("Contact email cannot exceed 150 characters.");

            RuleFor(c => c.ContactPhone)
                .NotEmpty().WithMessage("Contact phone is required.")
                .MaximumLength(20).WithMessage("Contact phone cannot exceed 20 characters.");
        }
    }
}