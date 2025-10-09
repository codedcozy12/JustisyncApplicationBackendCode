using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IJurisdictionRepository
    {
        Task<IEnumerable<Jurisdiction>> GetAllAsync();
        Task<Jurisdiction?> GetByIdAsync(Guid id);
        Task<IEnumerable<Jurisdiction>> GetBySubjectMatterAsync(string subjectMatter);
        Task<IEnumerable<Jurisdiction>> GetByCourtIdAsync(Guid courtId);
        Task<Jurisdiction> AddAsync(Jurisdiction jurisdiction);
        Task<Jurisdiction> UpdateAsync(Jurisdiction jurisdiction);
        Task<bool> DeleteAsync(Guid id);
    }
}