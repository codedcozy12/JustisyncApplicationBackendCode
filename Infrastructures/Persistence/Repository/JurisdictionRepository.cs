using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class JurisdictionRepository : IJurisdictionRepository
    {
        private readonly AppDbContext _context;
        public JurisdictionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Jurisdiction> AddAsync(Jurisdiction jurisdiction)
        {
            _context.Jurisdictions.Add(jurisdiction);
            return jurisdiction;
        }
        public async Task<IEnumerable<Jurisdiction>> GetAllAsync()
        {
            return await _context.Jurisdictions.Include(a => a.Courts).ToListAsync();
        }

        public async Task<IEnumerable<Jurisdiction>> GetByCourtIdAsync(Guid courtId)
        {
            return await _context.Jurisdictions
                .Where(j => j.Courts.Any(c => c.Id == courtId))
                .Include(j => j.Courts)
                .ToListAsync();
        }
        public async Task<Jurisdiction?> GetByIdAsync(Guid id)
        {
            return await _context.Jurisdictions
                .Include(j => j.Courts)
                .FirstOrDefaultAsync(j => j.Id == id);
        }
        public async Task<IEnumerable<Jurisdiction>> GetBySubjectMatterAsync(string subjectMatter)
        {
            return await _context.Jurisdictions
                .Where(j => j.SubjectMatter.Contains(subjectMatter))
                .Include(j => j.Courts)
                .ToListAsync();
        }

        public async Task<Jurisdiction> UpdateAsync(Jurisdiction jurisdiction)
        {
            _context.Jurisdictions.Update(jurisdiction);
            return jurisdiction;
        }
    }
}