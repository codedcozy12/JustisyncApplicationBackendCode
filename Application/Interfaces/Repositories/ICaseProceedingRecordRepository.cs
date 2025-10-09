using Domain.Entities;

namespace Application.Interfaces.Repositories
{
        public interface ICaseProceedingRecordRepository
        {
            Task<CaseProceedingRecord?> GetProceedingByIdAsync(Guid proceedingId);
            Task<IEnumerable<CaseProceedingRecord>> GetAllProceedingsAsync();
            Task AddProceedingAsync(CaseProceedingRecord newProceeding);
            Task UpdateProceedingAsync(CaseProceedingRecord updatedProceeding);
            Task DeleteProceedingAsync(Guid proceedingId);
            Task<IEnumerable<CaseProceedingRecord>> GetProceedingsByCaseAsync(Guid caseId);
            Task<IEnumerable<CaseProceedingRecord>> GetProceedingsByCourtSessionAsync(Guid courtSessionId);
        }
    }