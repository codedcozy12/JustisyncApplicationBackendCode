using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class CasePartyRepository : ICasePartyRepository
{
    private readonly AppDbContext _context;

    public CasePartyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CaseParty?> GetCasePartyByIdAsync(Guid casePartyId) =>
        await _context.CaseParties.FindAsync(casePartyId);

    public async Task<IEnumerable<CaseParty>> GetAllCasePartiesAsync() =>
        await _context.CaseParties.ToListAsync();

    public async Task AddCasePartyAsync(CaseParty newCaseParty)
    {
        _context.CaseParties.Add(newCaseParty);
    }
    public async Task UpdateCasePartyAsync(CaseParty updatedCaseParty)
    {
        _context.CaseParties.Update(updatedCaseParty);
    }
    public async Task DeleteCasePartyAsync(Guid casePartyId)
    {
        var party = await _context.CaseParties.FindAsync(casePartyId);
        if (party != null)
        {
            _context.CaseParties.Remove(party);
        }
    }
    public async Task<IEnumerable<CaseParty>> GetCasePartiesByCaseAsync(Guid caseId) =>
        await _context.CaseParties.Where(p => p.CaseId == caseId).ToListAsync();

    public async Task<IEnumerable<CaseParty>> GetCasePartiesByRoleAsync(PartyRole role)
    {
        return await _context.CaseParties.Where(p => p.Role == role).ToListAsync();
    }
}