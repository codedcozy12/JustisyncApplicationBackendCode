using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class CaseProceedingRecordRepository : ICaseProceedingRecordRepository
{
    private readonly AppDbContext _context;

    public CaseProceedingRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CaseProceedingRecord?> GetProceedingByIdAsync(Guid proceedingId) =>
        await _context.CaseProceedingRecords.FindAsync(proceedingId);

    public async Task<IEnumerable<CaseProceedingRecord>> GetAllProceedingsAsync() =>
        await _context.CaseProceedingRecords.ToListAsync();

    public async Task AddProceedingAsync(CaseProceedingRecord newProceeding)
    {
        _context.CaseProceedingRecords.Add(newProceeding);
    }
    public async Task UpdateProceedingAsync(CaseProceedingRecord updatedProceeding)
    {
        _context.CaseProceedingRecords.Update(updatedProceeding);
    }
    public async Task DeleteProceedingAsync(Guid proceedingId)
    {
        var proceeding = await _context.CaseProceedingRecords.FindAsync(proceedingId);
        if (proceeding != null)
        {
            _context.CaseProceedingRecords.Remove(proceeding);
        }
    }
    public async Task<IEnumerable<CaseProceedingRecord>> GetProceedingsByCaseAsync(Guid caseId) =>
        await _context.CaseProceedingRecords.Where(p => p.CaseId == caseId).ToListAsync();

    public async Task<IEnumerable<CaseProceedingRecord>> GetProceedingsByCourtSessionAsync(Guid courtSessionId) =>
        await _context.CaseProceedingRecords.Where(p => p.CourtSessionId == courtSessionId).ToListAsync();
}