using Application.Dto;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Service
{
    public interface IAuditLogService
    {
        Task<ResponseDto<IEnumerable<AuditLogDto>>> GetAllAuditLogsAsync();
        Task LogActionAsync(Guid userId, string action, string entityName, Guid entityId, string details, string role);
        Task<ResponseDto<IEnumerable<AuditLogDto>>> GetLogsByUserAsync(Guid userId);
        Task<ResponseDto<IEnumerable<AuditLogDto>>> GetLogsByRoleAsync(string role);
        Task<ResponseDto<IEnumerable<AuditLogDto>>> GetLogsByEntityAsync(string entityName, Guid entityId);
        Task<ResponseDto<IEnumerable<AuditLogDto>>> GetLogsByDateRangeAsync(DateTime start, DateTime end);
        Task<ResponseDto<IEnumerable<AuditLogDto>>> GetLogsByActionAsync(string action);
    }
}