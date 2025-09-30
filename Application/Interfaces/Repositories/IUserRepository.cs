using System.Linq.Expressions;
using Application.Dto;
using Domain.Entities;

namespace Application.Interface
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(Guid id);
        Task<User?> GetByIdAsync(Guid userId);
        Task<PageResponse<User>> GetAllAsync(PageRequest pageRequest);
        Task<List<User>> GetAllUsersWithoutPaginationAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid userId);
        Task<User?> GetByEmailAsync(string email);
        Task UpdatePasswordAsync(Guid userId, string newPasswordHash);
        Task<int> CountAsync(Expression<Func<User, bool>> expression);
    }
}