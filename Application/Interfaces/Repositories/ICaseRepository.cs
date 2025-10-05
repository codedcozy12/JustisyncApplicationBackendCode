using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories
{
    public interface ICaseRepository
    {
        Task<Case?> GetCaseByIdAsync(Guid caseId);
        Task<IEnumerable<Case>> GetAllCasesAsync();
        Task AddCaseAsync(Case newCase);
        Task UpdateCaseAsync(Case updatedCase);
        Task DeleteCaseAsync(Guid caseId);
        Task<IEnumerable<Case>> GetCasesByClientAsync(Guid clientId);
        Task<IEnumerable<Case>> GetCasesByLawyerAsync(Guid lawyerId);
        Task<IEnumerable<Case>> GetCasesByCourtAsync(Guid courtId);
        Task<IEnumerable<Case>> GetCasesByJudgeAsync(Guid judgeId);
        Task<Judge?> GetJudgeForCaseAsync(Guid caseId);
        Task<IEnumerable<JudgeCourt>> GetJudgeHistoryForCourtAsync(Guid courtId);
        Task<IEnumerable<Case>> GetCasesByOffenseTypeAsync(OffenseType offenseType);
        Task<IEnumerable<Case>> GetPendingCasesAsync();
        Task<IEnumerable<Case>> GetClosedCasesAsync();
    }
}