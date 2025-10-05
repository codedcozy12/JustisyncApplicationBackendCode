using Domain.Enums;

namespace Application.Dto
{
    public class AiChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid AIConversationSessionId { get; set; }
        public AIMessageSender Sender { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
    public class AichatMessageCreateRequestDto
    {
       public Guid AIConversationSessionId { get; set; }
       public AIMessageSender Sender { get; set; }
       public string Message { get; set; } = string.Empty;
    }
    public class AichatMessageUpdateRequestDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}