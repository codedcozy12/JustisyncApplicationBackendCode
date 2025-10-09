using Application.Dto;
using Domain.Enums;
using FluentValidation;

public class ChatMessageCreateRequestValidator : AbstractValidator<ChatMessageCreateRequestDto>
{
    public ChatMessageCreateRequestValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("Sender ID is required.");

        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("Receiver ID is required.");

        RuleFor(x => x.MessageType)
            .IsInEnum()
            .WithMessage("Invalid message type.");

        When(x => x.MessageType == MessageType.Text, () =>
        {
            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("Message text cannot be empty for text messages.")
                .MaximumLength(3000)
                .WithMessage("Message cannot exceed 3000 characters.");
        });

        When(x => x.MessageType != MessageType.Text, () =>
        {
            RuleFor(x => x.Message)
                .NotEmpty()
                .When(x => x.MessageType == MessageType.File)
                .WithMessage("File metadata or name is required when sending a file.");
        });
    }
}
