namespace Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; } = default!;
        public Guid SenderId { get; set; }
        public required User Sender { get; set; }
        public Guid ReceiverId { get; set; }
        public required User Receiver { get; set; }
        public required string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
