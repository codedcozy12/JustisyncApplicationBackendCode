using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly AppDbContext _context;
        public CertificateRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Certificate certificate)
        {
            await _context.Certificates.AddAsync(certificate);
        }
        public async Task DeleteAsync(Guid certificateId)
        {
            var certificate = await _context.Certificates.FindAsync(certificateId);
            if (certificate != null)
            {
                _context.Certificates.Remove(certificate);
            }
        }
        public async Task<IEnumerable<Certificate>> GetAllAsync()
        {
            return await _context.Certificates
                                 .Include(c => c.Lawyer)
                                 .ThenInclude(l => l.User)
                                 .ToListAsync();
        }

        public async Task<Certificate?> GetByFileNameAsync(string fileName)
        {
            return await _context.Certificates
                                 .Include(c => c.Lawyer)
                                 .ThenInclude(l => l.User)
                                 .FirstOrDefaultAsync(c => c.FilePath == fileName);
        }
        public async Task<Certificate?> GetByIdAsync(Guid certificateId)
        {
            return await _context.Certificates
                                 .Include(c => c.Lawyer)
                                 .ThenInclude(l => l.User)
                                 .FirstOrDefaultAsync(c => c.Id == certificateId);
        }
        public async Task<IEnumerable<Certificate>> GetByLawyerIdAsync(Guid lawyerId)
        {
            return await _context.Certificates
                                 .Include(c => c.Lawyer)
                                 .Where(c => c.LawyerId == lawyerId)
                                 .ToListAsync();
        }
        public async Task UpdateAsync(Certificate certificate)
        {
            var existingCertificate = await _context.Certificates.FindAsync(certificate.Id);
            if (existingCertificate != null)
            {
                existingCertificate.FilePath = certificate.FilePath;
                existingCertificate.LawyerId = certificate.LawyerId;
                _context.Certificates.Update(existingCertificate);
            }
        }
        public async Task<bool> VerifyCertificateAsync(Guid certificateId, bool isVerified)
        {
            var certificate = await _context.Certificates.FindAsync(certificateId);
            if (certificate != null)
            {
                certificate.IsVerified = isVerified;
                _context.Certificates.Update(certificate);
                return true;
            }
            return false;
        }
    }
}
