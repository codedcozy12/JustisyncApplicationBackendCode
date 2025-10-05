using Domain.Enums;

namespace Domain.Entities
{
    public class AiChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AIConversationSessionId { get; set; }
        public AiConversationSession AIConversationSession { get; set; } = default!;
        public AIMessageSender Sender { get; set; }
        public required string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}