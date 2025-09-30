using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ICourtRepository
    {
        Task<bool> CheckIfNumberExist(string number);
        Task<bool> CheckIfEmailExists(string email);
        Task<Court?> GetByIdAsync(Guid id);
        Task<IEnumerable<Court>> GetAllAsync();
        Task AddAsync(Court court);
        Task UpdateAsync(Court court);
        Task<IEnumerable<Court>> GetByRegistrarIdAsync(Guid registrarId);
        Task<IEnumerable<Court>> GetActiveCourtsAsync();
        Task<int> GetCourtsCount();
    }
}