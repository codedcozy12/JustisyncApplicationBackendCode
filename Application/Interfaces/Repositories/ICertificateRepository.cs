using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ICertificateRepository
    {
        Task<Certificate?> GetByIdAsync(Guid certificateId);
        Task<IEnumerable<Certificate>> GetAllAsync();
        Task AddAsync(Certificate certificate);
        Task UpdateAsync(Certificate certificate);
        Task DeleteAsync(Guid certificateId);
        Task<IEnumerable<Certificate>> GetByLawyerIdAsync(Guid lawyerId);
        Task<Certificate?> GetByFileNameAsync(string fileName);
        Task<bool> VerifyCertificateAsync(Guid certificateId, bool isVerified);
    }
}