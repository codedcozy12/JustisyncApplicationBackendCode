namespace Domain.Entities
{
    public class AIConversationSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public ICollection<AIChatMessage> Messages { get; set; } = [];
    }
}