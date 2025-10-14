using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class JudgeRepository : IJudgeRepository
    {
        private readonly AppDbContext _context;
        public JudgeRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Judge judge)
        {
            await _context.Judges.AddAsync(judge);
        }

        public async Task<bool> CheckIfEmailExists(string email)
        {
            return await _context.Judges
                                 .Include(j => j.User)
                                 .AnyAsync(j => j.User.Email == email);
        }

        public async Task<bool> CheckIfNumberExist(string number)
        {
            return await _context.Judges
                                 .Include(j => j.User)
                                 .AnyAsync(j => j.User.PhoneNumber == number
                                             && !j.IsDeleted
                                             && !j.User.IsDeleted);
        }

        public async Task DeleteAsync(Guid judgeId)
        {
            var judge = await _context.Judges.FindAsync(judgeId);
            if (judge != null)
            {
                _context.Judges.Remove(judge);
            }
        }
        public async Task<IEnumerable<Judge>> GetAllAsync()
        {
            return await _context.Judges
                                 .Include(j => j.User)
                                 .Include(Judge => Judge.JudgeCourts)
                                 .ToListAsync();
        }
        //public async Task<Judge?> GetByCaseIdAsync(Guid caseId)
        //{
            //return await _context.Judges
                                 //.Include(j => j.User)
                                 //.Include(j => j.Cases)
                                 //.FirstOrDefaultAsync(j => j.Cases.Any(c => c.Id == caseId));
        //}
        public async Task<IEnumerable<Judge>> GetByCourtIdAsync(Guid courtId)
        {
            return await _context.Judges
                                 .Include(j => j.User)
                                 .Include(a => a.JudgeCourts)
                                 .Where(j => j.JudgeCourts.Any(a => a.CourtId== courtId))
                                 .ToListAsync();
        }
        public async Task<Judge?> GetByIdAsync(Guid judgeId)
        {
            return await _context.Judges
                                 .Include(j => j.User)
                                 .Include(a => a.JudgeCourts)
                                  .ThenInclude(a => a.Court)
                                 .FirstOrDefaultAsync(j => j.Id == judgeId);
        }
        public async Task<Judge?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Judges
                                 .Include(j => j.User)
                                 .FirstOrDefaultAsync(j => j.UserId == userId);
        }
        public async Task UpdateAsync(Judge judge)
        {
            _context.Judges.Update(judge);
            await Task.CompletedTask;
        }

        public async Task<Judge?> GetJudgeByCourtIdAsync(Guid courtId)
        {
            return await _context.Judges.Include(a => a.JudgeCourts)
                .FirstOrDefaultAsync(j => j.JudgeCourts.Any(a => a.CourtId == courtId && a.IsActive == true));
        }
    }
}