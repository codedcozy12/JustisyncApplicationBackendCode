using System;
using Domain.Entities;
using FluentValidation;

namespace Application.Validators
{
    public class ChatCreateRequestValidator : AbstractValidator<Chat>
    {
        public ChatCreateRequestValidator() 
        {
             RuleFor(x => x.RecipientId)
            .NotEmpty().WithMessage("RecipientId is required.")
            .NotEqual(Guid.Empty).WithMessage("RecipientId must be a valid GUID.")
            .NotEqual(x => x.InitiatorId)
            .WithMessage("Initiator and Recipient cannot be the same user.");
        }
    }
}