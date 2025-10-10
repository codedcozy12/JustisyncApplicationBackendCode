using Application.Dto;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class CaseRepository : ICaseRepository
    {
        private readonly AppDbContext _context;
        public CaseRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddCaseAsync(Case newCase)
        {
            await _context.Cases.AddAsync(newCase);
        }
        public async Task DeleteCaseAsync(Guid caseId)
        {
            var caseToDelete = await _context.Cases.FindAsync(caseId);
            if (caseToDelete != null)
            {
                _context.Cases.Remove(caseToDelete);
            }
        }
        public async Task<IEnumerable<Case>> GetAllCasesAsync()
        {
            return await _context.Cases
                .Include(c => c.Lawyer)
                .Include(c => c.Client)
                .Include(c => c.OffenseLocation)
                .Include(c => c.Court)
                .ToListAsync();
        }
        public async Task<Case?> GetCaseByIdAsync(Guid caseId)
        {
            return await _context.Cases
            .Include(c => c.Lawyer)
            .Include(c => c.Client)
            .Include(c => c.OffenseLocation)
            .Include(a => a.CaseParties)
            .Include(c => c.Court)
                .ThenInclude(court => court.JudgeCourts)
                    .ThenInclude(jc => jc.Judge)
            .FirstOrDefaultAsync(c => c.Id == caseId);
        }
    public async Task<IEnumerable<Case>> GetCasesByClientAsync(Guid clientId)
        {
            return await _context.Cases
                .Where(c => c.ClientId == clientId)
                .Include(c => c.Lawyer)
                .Include(c => c.OffenseLocation)
                .Include(c => c.Court)
                .ToListAsync();
        }
        public async Task<IEnumerable<Case>> GetCasesByCourtAsync(Guid courtId)
        {
            return await _context.Cases
                .Where(c => c.CourtId == courtId)
                .Include(c => c.Lawyer)
                .Include(c => c.Client)
                .ToListAsync();
        }
        public async Task<IEnumerable<Case>> GetCasesByJudgeAsync(Guid judgeId)
        {
            return await _context.Cases
                .Include(c => c.Court)
                    .ThenInclude(court => court.JudgeCourts)
                .Where(c => c.Court.JudgeCourts.Any(jc => jc.JudgeId == judgeId))
                .Include(c => c.Lawyer)
                .Include(c => c.Client)
                .ToListAsync();
        }
        public async Task<IEnumerable<Case>> GetCasesByLawyerAsync(Guid lawyerId)
        {
            return await _context.Cases
                .Where(c => c.LawyerId == lawyerId)
                .Include(c => c.Client)
                .Include(c => c.Court)
                .ToListAsync();
        }
        public async Task<IEnumerable<Case>> GetCasesByOffenseTypeAsync(OffenseType offenseType)
        {
            return await _context.Cases
                .Where(c => c.OffenseType == offenseType)
                .Include(c => c.Lawyer)
                .Include(c => c.Client)
                .Include(c => c.Court)
                .ToListAsync();
        }
        public async Task<IEnumerable<Case>> GetClosedCasesAsync()
        {
            return await _context.Cases
                .Where(c => c.Status == CaseStatus.Closed)
                .Include(c => c.Lawyer)
                .Include(c => c.Client)
                .Include(c => c.Court)
                .ToListAsync();
        }
        public async Task<Judge?> GetJudgeForCaseAsync(Guid caseId)
        {
            var caseEntity = await _context.Cases
                .Include(c => c.Court)
                    .ThenInclude(court => court.JudgeCourts)
                        .ThenInclude(jc => jc.Judge)
                .FirstOrDefaultAsync(c => c.Id == caseId);

            return caseEntity?.Court.JudgeCourts
                .OrderByDescending(jc => EF.Property<DateTime>(jc, "AssignedAt"))
                .Select(jc => jc.Judge)
                .FirstOrDefault();
        }
        public async Task<IEnumerable<JudgeCourt>> GetJudgeHistoryForCourtAsync(Guid courtId)
        {
            return await _context.JudgeCourts
                .Where(jc => jc.CourtId == courtId)
                .Include(jc => jc.Judge)
                .OrderByDescending(jc => EF.Property<DateTime>(jc, "AssignedAt"))
                .ToListAsync();
        }
        public async Task<IEnumerable<Case>> GetPendingCasesAsync()
        {
            return await _context.Cases
                .Where(c => c.Status == CaseStatus.Initiated)
                .Include(c => c.Lawyer)
                .Include(c => c.Client)
                .Include(c => c.Court)
                .ToListAsync();
        }

        public async Task<IEnumerable<Case>> GetPendingVerificationCaseAsync(PageRequest pageRequest)
        {
            return await _context.Cases
                .Where(c => c.ApprovalStatus == CaseApprovalStatus.pending)
                .Include(c => c.Lawyer)
                .Include(c => c.Client)
                .Include(c => c.Court)
                .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .ToListAsync();
        }

        public async Task UpdateCaseAsync(Case updatedCase)
        {
            _context.Cases.Update(updatedCase);
        }
    }
}