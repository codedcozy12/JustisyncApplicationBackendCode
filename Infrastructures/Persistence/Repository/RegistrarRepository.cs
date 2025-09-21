using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class RegistrarRepository : IRegistrarRepository
    {
        private readonly AppDbContext _context;
        public RegistrarRepository(AppDbContext context)
        {
           _context = context;
        }
        public async Task AddAsync(Registrar registrar)
        {
            await _context.Registrars.AddAsync(registrar);
        }
        public async Task DeleteAsync(Guid registrarId)
        {
            var registrar = await _context.Registrars.FindAsync(registrarId);
            if (registrar != null)
            {
                _context.Registrars.Remove(registrar);
            }
        }
        public async Task<IEnumerable<Registrar>> GetAllAsync()
        {
            return await _context.Registrars
                                 .Include(r => r.User)
                                 .Include(r => r.Court)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<Registrar>> GetByCourtIdAsync(Guid courtId)
        {
            return await _context.Registrars
                                 .Include(r => r.User)
                                 .Include(r => r.Court)
                                 .Where(r => r.CourtId == courtId)
                                 .ToListAsync();
        }
        public async Task<Registrar?> GetByIdAsync(Guid registrarId)
        {
            return await _context.Registrars.Include(r => r.User).Include(r => r.Court).FirstOrDefaultAsync(r => r.Id == registrarId);
        }
        public async Task<Registrar?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Registrars
                                 .Include(r => r.User)
                                 .Include(r => r.Court)
                                 .FirstOrDefaultAsync(r => r.UserId == userId);
        }
        public async Task UpdateAsync(Registrar registrar)
        {
            _context.Registrars.Update(registrar);
        }
    }
}