using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ICourtSessionRepository
    {
        Task<CourtSession?> GetByIdAsync(Guid id);
        Task<IEnumerable<CourtSession>> GetAllAsync();
        Task<IEnumerable<CourtSession>> GetByCourtAsync(Guid courtId);
        Task<IEnumerable<CourtSession>> GetByCaseAsync(Guid caseId);
        Task AddAsync(CourtSession courtSession);
        Task UpdateAsync(CourtSession courtSession);
        Task<IEnumerable<CourtSession>> GetUpcomingSessionsAsync(DateTime from, DateTime to);
    }
}