using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IAiChatMessageRepository
    {
        Task<AiChatMessage?> GetByIdAsync(Guid id);
        Task<IEnumerable<AiChatMessage>> GetAllAsync();
        Task AddAsync(AiChatMessage message);
        Task UpdateAsync(AiChatMessage message);
        Task<IEnumerable<AiChatMessage>> GetMessagesBySessionAsync(Guid sessionId);
        Task<IEnumerable<AiChatMessage>> GetMessagesByUserAsync(Guid userId);
        Task<IEnumerable<AiChatMessage>> GetMessagesInTimeRangeAsync(Guid sessionId, DateTime start, DateTime end);
        Task<AiChatMessage?> GetLastMessageInSessionAsync(Guid sessionId);
    }
}