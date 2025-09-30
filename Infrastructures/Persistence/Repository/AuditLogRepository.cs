using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Repository
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;
        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAuditLogAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
        }

        public async Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync()
        {
            return await _context.AuditLogs.ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByUserIdAsync(Guid userId)
        {
            return await _context.AuditLogs
                                 .Where(log => log.UserId == userId)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<AuditLog>> GetLogsByActionAsync(string action)
        {
            return await _context.AuditLogs
                                 .Where(log => log.Action == action)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<AuditLog>> GetLogsByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.AuditLogs
                                 .Where(log => log.Timestamp >= start && log.Timestamp <= end)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, Guid entityId)
        {
            return await _context.AuditLogs
                                 .Where(log => log.EntityName == entityName && log.EntityId == entityId)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<AuditLog>> GetLogsByRoleAsync(string role)
        {
            return await _context.AuditLogs
                                 .Where(log => log.Role == role)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<AuditLog>> GetLogsByUserAsync(Guid userId)
        {
            return await _context.AuditLogs
                                 .Where(log => log.UserId == userId)
                                 .ToListAsync();
        }
    }
}