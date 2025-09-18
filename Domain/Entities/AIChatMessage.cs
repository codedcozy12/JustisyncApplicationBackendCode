using Domain.Enums;

namespace Domain.Entities
{
    public class AIChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SessionId { get; set; }
        public AIConversationSession Session { get; set; } = default!;
        public AIMessageSender Sender { get; set; }
        public required string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}