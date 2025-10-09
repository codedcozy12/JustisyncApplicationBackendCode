using Domain.Entities;

namespace Application.Dto
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public Guid InitiatorId { get; set; }
        public Guid RecipientId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessageAt { get; set; }
        public ICollection<ChatMessageDto> Messages { get; set; } = [];
    }
    public class ChatCreateRequestDto
    {
        public Guid InitiatorId { get; set; }
        public Guid RecipientId { get; set; }
    }
}