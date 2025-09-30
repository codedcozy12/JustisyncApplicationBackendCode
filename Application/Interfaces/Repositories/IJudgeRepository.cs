using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IJudgeRepository
    {
        Task<bool> CheckIfNumberExist(string number);
        Task<bool> CheckIfEmailExists(string email);
        Task<Judge?> GetByIdAsync(Guid judgeId);
        Task<IEnumerable<Judge>> GetAllAsync();
        Task AddAsync(Judge judge);
        Task UpdateAsync(Judge judge);
        Task DeleteAsync(Guid judgeId);
        Task<Judge?> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Judge>> GetByCourtIdAsync(Guid courtId);
        Task<Judge?> GetJudgeByCourtIdAsync(Guid courtId);
        //Task<Judge?> GetByCaseIdAsync(Guid caseId);
    }
}