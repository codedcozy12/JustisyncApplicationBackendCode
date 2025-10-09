using FluentValidation;

namespace Application.Validators
{
    public class DocumentCreateRequestValidator : AbstractValidator<DocumentCreateRequestDto>
    {
        public DocumentCreateRequestValidator()
        {
            RuleFor(d => d.CaseId)
            .NotEmpty()
            .WithMessage("Case ID is required. Every document must be linked to a case.");

            RuleFor(d => d.FileName)
                .NotEmpty()
                .WithMessage("File name is required.")
                .MaximumLength(150)
                .WithMessage("File name must not exceed 150 characters.");

            RuleFor(d => d.FileType)
                .NotEmpty()
                .WithMessage("File type is required.")
                .Must(type => new[] { "pdf", "docx", "jpg", "png", "txt" }
                    .Contains(type.ToLower()))
                .WithMessage("Invalid file type. Supported types are: pdf, docx, jpg, png, txt.");
            
            RuleFor(d => d.FileUrl)
            .NotNull().WithMessage("File is required.")
            .Must(f => f.Length > 0).WithMessage("File cannot be empty.")
            .Must(f => f.Length <= 10 * 1024 * 1024).WithMessage("File size cannot exceed 10MB.")
            .Must(f => f.ContentType == "application/pdf" || f.ContentType.StartsWith("image/"))
            .WithMessage("Only PDF or image files are allowed.");
        }
    }
}