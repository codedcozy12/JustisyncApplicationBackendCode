using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IChatMessageRepository
    {
        Task<IEnumerable<ChatMessage>> GetMessagesByChatIdAsync(Guid chatId);
        Task AddMessageAsync(ChatMessage message);
        Task<ChatMessage> GetMessageByIdAsync(Guid messageId);
        Task<IEnumerable<ChatMessage>> GetAllChatMessages(Guid chatId);
    }
}