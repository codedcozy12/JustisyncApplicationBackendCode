namespace Domain.Entities
{
    public class Chat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastMessageAt { get; set;} = DateTime.UtcNow;
        public ICollection<ChatMessage> Messages { get; set; } = [];
    }
}
