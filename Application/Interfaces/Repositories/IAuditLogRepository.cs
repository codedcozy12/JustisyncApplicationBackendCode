using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAuditLogAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetAuditLogsByUserIdAsync(Guid userId);
        Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync();
        Task<IEnumerable<AuditLog>> GetLogsByUserAsync(Guid userId);
        Task<IEnumerable<AuditLog>> GetLogsByRoleAsync(string role);
        Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, Guid entityId);
        Task<IEnumerable<AuditLog>> GetLogsByDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<AuditLog>> GetLogsByActionAsync(string action);
    }
}