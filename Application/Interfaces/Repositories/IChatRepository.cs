using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task<IEnumerable<Chat>> GetAllChatsAsync();
        Task<Chat?> GetChatByIdAsync(Guid chatId);
        Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId);
        Task AddChatAsync(Chat chat);
        Task UpdateChatAsync(Chat chat);
        Task DeleteChatAsync(Guid chatId);
    }
}