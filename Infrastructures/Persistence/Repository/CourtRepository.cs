using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class CourtRepository : ICourtRepository
    {
        private readonly AppDbContext _context;
        public CourtRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Court court)
        {
            await _context.Courts.AddAsync(court);
        }

        public async Task<bool> CheckIfEmailExists(string email)
        {
            return await _context.Courts.AnyAsync(c => c.ContactEmail == email);
        }

        public async Task<bool> CheckIfNumberExist(string number)
        {
            return await _context.Courts.AnyAsync(c => c.ContactPhone == number);
        }

        public async Task<IEnumerable<Court>> GetActiveCourtsAsync()
        {
            return await _context.Courts
                                 .Where(c => c.IsActive)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<Court>> GetAllAsync()
        {
            return await _context.Courts.ToListAsync();
        }
        public async Task<Court?> GetByIdAsync(Guid id)
        {
            return await _context.Courts
                                 .Include(c => c.Registrars)
                                 .Include(c => c.JudgeCourts)
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<IEnumerable<Court>> GetByRegistrarIdAsync(Guid registrarId)
        {
            return await _context.Courts
                                 .Where(c => c.Registrars.Any(r => r.Id == registrarId))
                                 .ToListAsync();
        }

        public async Task<int> GetCourtsCount()
        {
            return await _context.Courts.CountAsync();
        }

        public async Task<Court?> GetJudgeActiveCourt(Guid judgeId)
        {
            return await _context.Courts
                                 .Include(c => c.JudgeCourts)
                                 .ThenInclude(jc => jc.Judge)
                                 .FirstOrDefaultAsync(c => c.JudgeCourts.Any(jc => jc.JudgeId == judgeId && jc.IsActive));
        }

        public async Task UpdateAsync(Court court)
        {
            _context.Courts.Update(court);
            await Task.CompletedTask;
        }
    }
}