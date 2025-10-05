using Domain.Entities;

namespace Application.Dto
{
    public class AiConversationSessionDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartedAt { get; set; }
        public ICollection<AiChatMessageDto> Messages { get; set; } = [];
    }
    public class AiConversationSessionCreateRequestDto
    {
        public Guid UserId { get; set; }
    }
    public class AiConversationSessionUpdateRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}