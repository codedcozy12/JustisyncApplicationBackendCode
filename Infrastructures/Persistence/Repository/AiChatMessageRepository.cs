using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class AiChatMessageRepository : IAiChatMessageRepository
    {
        private readonly AppDbContext _context;

        public AiChatMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AiChatMessage?> GetByIdAsync(Guid id)
        {
            return await _context.AIChatMessages
                .Include(m => m.AIConversationSession)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<IEnumerable<AiChatMessage>> GetAllAsync()
        {
            return await _context.AIChatMessages
                .Include(m => m.AIConversationSession)
                .ToListAsync();
        }
        public async Task AddAsync(AiChatMessage message)
        {
            await _context.AIChatMessages.AddAsync(message);
        }
        public async Task UpdateAsync(AiChatMessage message)
        {
            _context.AIChatMessages.Update(message);
        }
        public async Task<IEnumerable<AiChatMessage>> GetMessagesBySessionAsync(Guid sessionId)
        {
            return await _context.AIChatMessages
                .Where(m => m.AIConversationSessionId == sessionId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<AiChatMessage>> GetMessagesByUserAsync(Guid userId)
        {
            return await _context.AIChatMessages
                .Where(m => m.AIConversationSession.UserId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<AiChatMessage>> GetMessagesInTimeRangeAsync(Guid sessionId, DateTime start, DateTime end)
        {
            return await _context.AIChatMessages
                .Where(m => m.AIConversationSessionId == sessionId &&
                            m.SentAt >= start && m.SentAt <= end)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
        public async Task<AiChatMessage?> GetLastMessageInSessionAsync(Guid sessionId)
        {
            return await _context.AIChatMessages
                .Where(m => m.AIConversationSessionId == sessionId)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        }
    }
}