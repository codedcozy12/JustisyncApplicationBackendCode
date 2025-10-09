using Domain.Enums;

namespace Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
        public Guid SenderId { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public Guid ReceiverId { get; set; }
        public required string Message { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}