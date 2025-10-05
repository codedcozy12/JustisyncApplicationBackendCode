using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class AiConversationSessionRepository : IAiConversationSessionRepository
    {
        private readonly AppDbContext _context;

        public AiConversationSessionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AiConversationSession?> GetByIdAsync(Guid id)
        {
            return await _context.AIConversationSessions
                .Include(s => s.Messages)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<IEnumerable<AiConversationSession>> GetByUserAsync(Guid userId)
        {
            return await _context.AIConversationSessions
                .Where(s => s.UserId == userId)
                .Include(s => s.Messages)
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();
        }
        public async Task AddAsync(AiConversationSession session)
        {
            await _context.AIConversationSessions.AddAsync(session);
        }
        public async Task UpdateAsync(AiConversationSession session)
        {
            _context.AIConversationSessions.Update(session);
        }

        public async Task<IEnumerable<AiConversationSession>> GetAllAsync()
        {
            return await _context.AIConversationSessions
                .Include(s => s.Messages)
                .Include(s => s.User)
                .ToListAsync();
        }
    }
}