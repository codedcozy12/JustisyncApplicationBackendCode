using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface IAiConversationSessionRepository
{
    Task<AiConversationSession?> GetByIdAsync(Guid id);
    Task<IEnumerable<AiConversationSession>> GetAllAsync();
    Task<IEnumerable<AiConversationSession>> GetByUserAsync(Guid userId);
    Task AddAsync(AiConversationSession session);
    Task UpdateAsync(AiConversationSession session);
}