using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly AppDbContext _context;
        public ChatMessageRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddMessageAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
            var chat = await _context.Chats.FindAsync(message.ChatId);
            if (chat != null)
            {
                chat.LastMessageAt = DateTime.UtcNow;
                _context.Chats.Update(chat);
            }
        }
        public async Task<IEnumerable<ChatMessage>> GetAllChatMessages(Guid chatId)
        {
            return await _context.ChatMessages
           .Include(m => m.Sender)
           .Include(m => m.Receiver)
           .Where(m => m.ChatId == chatId)
           .OrderBy(m => m.SentAt)
           .ToListAsync();
        }

        public async Task<ChatMessage> GetMessageByIdAsync(Guid messageId)
        {
            return await _context.ChatMessages
            .Include(m => m.Chat)
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .FirstOrDefaultAsync(m => m.Id == messageId )
            ?? throw new KeyNotFoundException($"ChatMessage with Id {messageId} not found");
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesByChatIdAsync(Guid chatId)
        {
            return await _context.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
        }
    }
}