using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IRegistrarRepository
    {
        Task<Registrar?> GetByIdAsync(Guid registrarId);
        Task<IEnumerable<Registrar>> GetAllAsync();
        Task AddAsync(Registrar registrar);
        Task UpdateAsync(Registrar registrar);
        Task DeleteAsync(Guid registrarId);
        Task<Registrar?> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Registrar>> GetByCourtIdAsync(Guid courtId);
    }
}