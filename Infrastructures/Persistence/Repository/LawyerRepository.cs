using System.Linq.Expressions;
using Application.Dto;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class LawyerRepository : ILawyerRepository
    {
        private readonly AppDbContext _context;
        public LawyerRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Lawyer lawyer)
        {
            await _context.Lawyers.AddAsync(lawyer);
        }
        public async Task DeleteAsync(Guid lawyerId)
        {
            var lawyer = await _context.Lawyers.FindAsync(lawyerId);
            if (lawyer != null)
            {
                _context.Lawyers.Remove(lawyer);
            }
        }
        public async Task<IEnumerable<Lawyer>> GetAllAsync()
        {
            return await _context.Lawyers
                                 .Include(l => l.User)
                                 .Include(l => l.Certificates)
                                 .ToListAsync();
        }
        public async Task<Lawyer?> GetByIdAsync(Guid lawyerId)
        {
            return await _context.Lawyers
                                 .Include(l => l.User)
                                    .Include(l => l.Certificates)
                                 .FirstOrDefaultAsync(l => l.Id == lawyerId);
        }
        public async Task<IEnumerable<Lawyer>> GetBySpecializationAsync(LawyerSpecialization specialization)
        {
            return await _context.Lawyers
                                 .Include(l => l.User)
                                 .Where(l => l.LawyerSpecialization == specialization)
                                 .ToListAsync();
        }
        public async Task<Lawyer?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Lawyers
                                 .Include(l => l.User)
                                 .Include (l => l.Certificates)
                                 .FirstOrDefaultAsync(l => l.UserId == userId);
        }
        public async Task<IEnumerable<Lawyer>> GetVerifiedLawyersAsync()
        {
            return await _context.Lawyers
                                 .Include(l => l.User)
                                 .Include(l => l.Certificates)
                                 .Where(l => l.VerificationStatus == lawyerVerificationStatus.Verified)
                                 .ToListAsync();
        }

        public async Task<PageResponse<Lawyer>> GetPendingLawyersAsync(PageRequest pageRequest)
        {
            var query =  _context.Lawyers
                                 .Include(l => l.User)
                                 .Include(l => l.Certificates)
                                 .Where(l => l.VerificationStatus == lawyerVerificationStatus.pending);
            var lawyers = await query.OrderByDescending(l => l.DateCreated)
                               .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                               .Take(pageRequest.PageSize).ToListAsync();

            return new PageResponse<Lawyer> 
            {
                PageNumber = pageRequest.PageNumber,
                Items = lawyers ,
                PageSize = pageRequest.PageSize,
                TotalCount = query.Count()
            };
        }
        public async Task<PageResponse<Lawyer>> SearchAsync(Expression<Func<Lawyer,bool>> exp, PageRequest request)
        {
            var query = _context.Lawyers
                              .Include(l => l.User)
                              .Include(l => l.Certificates)
                              .Where(exp);
                                
            var totalRecord = query.Count();
            var items = await query.Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize).ToListAsync();

            return new PageResponse<Lawyer>
            {
                TotalCount = totalRecord,
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }
        public async Task UpdateAsync(Lawyer lawyer)
        {
            _context.Lawyers.Update(lawyer);
            await Task.CompletedTask;
        }

        public async Task<bool> CheckIfNumberExist(string number)
        {
            return await _context.Lawyers
                                 .Include(l => l.User)
                                 .AnyAsync(l => l.User.PhoneNumber == number);
        }

        public async Task<bool> CheckIfEmailExists(string email)
        {
            return await _context.Lawyers
                                 .Include(l => l.User)
                                 .AnyAsync(l => l.User.Email == email);
        }
    }
}