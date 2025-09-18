using System.Linq.Expressions;
using Application.Dto;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories
{
    public interface ILawyerRepository
    {
        Task<PageResponse<Lawyer>> GetPendingLawyersAsync(PageRequest pageRequest);
        Task<Lawyer?> GetByIdAsync(Guid lawyerId);
        Task<IEnumerable<Lawyer>> GetAllAsync();
        Task AddAsync(Lawyer lawyer);
        Task UpdateAsync(Lawyer lawyer);
        Task DeleteAsync(Guid lawyerId);
        Task<Lawyer?> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Lawyer>> GetVerifiedLawyersAsync();
        Task<IEnumerable<Lawyer>> GetBySpecializationAsync(LawyerSpecialization specialization);
        Task<PageResponse<Lawyer>> SearchAsync(Expression<Func<Lawyer, bool>> exp, PageRequest request);
    }
}