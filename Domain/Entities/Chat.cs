namespace Domain.Entities
{
    public class Chat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LawyerId { get; set; }
        public User Lawyer { get; set; }
        public User Client { get; set; }
        public Guid ClientId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastMessageAt { get; set;} = DateTime.UtcNow;
        public ICollection<ChatMessage> Messages { get; set; } = [];
    }
}
