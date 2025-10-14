using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class CourtSessionRepository : ICourtSessionRepository
    {
        private readonly AppDbContext _context;
        public CourtSessionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(CourtSession courtSession)
        {
            await _context.CourtSessions.AddAsync(courtSession);
        }
        public async Task<IEnumerable<CourtSession>> GetAllAsync()
        {
            return await _context.CourtSessions
           .Include(cs => cs.Case)
           .Include(cs => cs.Court)
           .Include(cs => cs.Proceedings)
           .ToListAsync();
        }
        public async Task<IEnumerable<CourtSession>> GetByCaseAsync(Guid caseId)
        {
            return await _context.CourtSessions
                .Where(cs => cs.CaseId == caseId)
                .Include(cs => cs.Court)
                .Include(cs => cs.Proceedings)
                .ToListAsync();
        }
        public async Task<IEnumerable<CourtSession>> GetByCourtAsync(Guid courtId)
        {
            return await _context.CourtSessions
            .Where(cs => cs.CourtId == courtId)
            .Include(cs => cs.Case)
            .Include(cs => cs.Proceedings)
            .ToListAsync();
        }
        public async Task<CourtSession?> GetByIdAsync(Guid id)
        {

            return await _context.CourtSessions
                .Include(cs => cs.Case)
                .Include(cs => cs.Court)
                .Include(cs => cs.Proceedings)
                .Include(cs => cs.Users)
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }

        public async Task<CourtSession?> GetByMeetingRoomAsync(string roomName)
        {
            return await _context.CourtSessions
                .Include(cs => cs.Case)
                .Include(cs => cs.Court)
                .Include(cs => cs.Proceedings)
                .FirstOrDefaultAsync(cs => cs.VirtualMeetingRoom == roomName);
        }

        public async Task<IEnumerable<CourtSession>> GetUpcomingSessionsAsync(DateTime from, DateTime to)
        {
            return await _context.CourtSessions
                .Where(cs => cs.ScheduledDate >= from && cs.ScheduledDate <= to)
                .Include(cs => cs.Case)
                .Include(cs => cs.Court)
                .ToListAsync();
        }
        public async Task UpdateAsync(CourtSession courtSession)
        {
            _context.CourtSessions.Update(courtSession);
            await Task.CompletedTask;
        }
    }
}