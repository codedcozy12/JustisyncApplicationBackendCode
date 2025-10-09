using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories
{
        public interface ICasePartyRepository
        {
            Task<CaseParty?> GetCasePartyByIdAsync(Guid casePartyId);
            Task<IEnumerable<CaseParty>> GetAllCasePartiesAsync();
            Task AddCasePartyAsync(CaseParty newCaseParty);
            Task UpdateCasePartyAsync(CaseParty updatedCaseParty);
            Task DeleteCasePartyAsync(Guid casePartyId);
            Task<IEnumerable<CaseParty>> GetCasePartiesByCaseAsync(Guid caseId);
            Task<IEnumerable<CaseParty>> GetCasePartiesByRoleAsync(PartyRole role);
        }
    }