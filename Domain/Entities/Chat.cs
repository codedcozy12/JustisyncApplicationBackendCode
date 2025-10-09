namespace Domain.Entities
{
    public class Chat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InitiatorId { get; set; }
        public Guid  RecipientId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastMessageAt { get; set;} = DateTime.UtcNow;
        public ICollection<ChatMessage> Messages { get; set; } = [];
    }
}