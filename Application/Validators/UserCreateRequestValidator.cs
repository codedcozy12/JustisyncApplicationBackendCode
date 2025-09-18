using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class UserCreateRequestValidator : AbstractValidator<UserCreateRequestDto>
    {
        public UserCreateRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                .Matches("^[a-zA-Z0-9]*$").WithMessage("Username must only contain letters and numbers (no special characters).")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.MiddleName)
                .MaximumLength(50).WithMessage("Middle name must not exceed 50 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{9,14}$")
                .WithMessage("Phone number must be valid and contain between 10 and 15 digits.");

            RuleFor(x => x.ProfilePictureUrl)
            .Must(file => file == null || file.Length > 0)
        .WithMessage("Uploaded file cannot be empty.")
         .Must(file => file == null || file.Length <= 2 * 1024 * 1024)  
        .WithMessage("Profile picture must not exceed 2MB.")
        .Must(file => file == null ||
        new[] { ".jpg", ".jpeg", ".png" }
        .Contains(Path.GetExtension(file.FileName).ToLower()))
        .WithMessage("Only .jpg, .jpeg, or .png files are allowed.")
        .Must(file => file == null ||
        (file.ContentType == "image/jpeg" || file.ContentType == "image/png"))
        .WithMessage("Only JPEG or PNG images are allowed.");
        }
    }
}