using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;
        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddChatAsync(Chat chat)
        {
            await _context.Chats.AddAsync(chat);
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            var chat = await _context.Chats.FindAsync(chatId);
            if (chat != null)
            {
                _context.Chats.Remove(chat);
            }
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _context.Chats
            .Include(c => c.User)
            .Include(c => c.Messages)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();
        }

        public async Task<Chat?> GetChatByIdAsync(Guid chatId)
        {
             return await _context.Chats
            .Include(c => c.User)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId)
        {
             return await _context.Chats
             .Include(c => c.Messages)
             .Where(c => c.InitiatorId == userId)
             .OrderByDescending(c => c.LastMessageAt)
             .ToListAsync();
        }
        public async Task UpdateChatAsync(Chat chat)
        {
            _context.Chats.Update(chat);
        }
    }
}
