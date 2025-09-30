using Application.Dto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<AuditLogService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AuditLogService(IAuditLogRepository auditLogRepository, ILogger<AuditLogService> logger,IUnitOfWork unitOfWork)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDto<IEnumerable<AuditLogDto>>> GetAllAuditLogsAsync()
        {
            {
                var logs = await _auditLogRepository.GetAllAuditLogsAsync();
                var logDtos = logs.Select(log => new AuditLogDto
                {
                    Id = log.Id,
                    UserId = log.UserId,
                    Action = log.Action,
                    EntityName = log.EntityName,
                    EntityId = log.EntityId,
                    Timestamp = log.Timestamp,
                    Role = log.Role.ToString(),
                    Details = log.Details
                });
                return new ResponseDto<IEnumerable<AuditLogDto>>
                {
                    Data = logDtos,
                    IsSuccess = true,
                    Message = "Audit logs retrieved successfully"
                };
            }
        }

        public async Task<ResponseDto<IEnumerable<AuditLogDto>>> GetLogsByActionAsync(string action)
        {
            var logs = await _auditLogRepository.GetLogsByActionAsync(action);
            var logDtos = logs.Select(log => new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                Timestamp = log.Timestamp,
                Role = log.Role.ToString(),
                Details = log.Details
            });
            return new ResponseDto<IEnumerable<AuditLogDto>>
            {
                Data = logDtos,
                IsSuccess = true,
                Message = "Audit logs retrieved successfully"
            };
        }
        public async Task<ResponseDto<IEnumerable<AuditLogDto>>> GetLogsByDateRangeAsync(DateTime start, DateTime end)
        {
            var logs = await _auditLogRepository.GetLogsByDateRangeAsync(start, end);
            var logDtos = logs.Select(log => new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                Timestamp = log.Timestamp,
                Role = log.Role.ToString(),
                Details = log.Details
            });
            return new ResponseDto<IEnumerable<AuditLogDto>>
            {
                Data = logDtos,
                IsSuccess = true,
                Message = "Audit logs retrieved successfully"
            };
        }
        public async Task<ResponseDto<IEnumerable<AuditLogDto>>> GetLogsByEntityAsync(string entityName, Guid entityId)
        {
            var logs = await _auditLogRepository.GetLogsByEntityAsync(entityName, entityId);
            var logDtos = logs.Select(log => new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                Timestamp = log.Timestamp,
                Role = log.Role.ToString(),
                Details = log.Details
            });
            return new ResponseDto<IEnumerable<AuditLogDto>>
            {
                Data = logDtos,
                IsSuccess = true,
                Message = "Audit logs retrieved successfully"
            };
        }
        public async Task<ResponseDto<IEnumerable<AuditLogDto>>> GetLogsByRoleAsync(string role)
        {
            var logs = await _auditLogRepository.GetLogsByRoleAsync(role);
            var logDtos = logs.Select(log => new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                Timestamp = log.Timestamp,
                Role = log.Role.ToString(),
                Details = log.Details
            });
            return new ResponseDto<IEnumerable<AuditLogDto>>
            {
                Data = logDtos,
                IsSuccess = true,
                Message = "Audit logs retrieved successfully"
            };
        }

        public async Task<ResponseDto<IEnumerable<AuditLogDto>>> GetLogsByUserAsync(Guid userId)
        {
            var logs = await _auditLogRepository.GetLogsByUserAsync(userId);
            var logDtos = logs.Select(log => new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                Timestamp = log.Timestamp,
                Role = log.Role.ToString(),
                Details = log.Details
            });
            return new ResponseDto<IEnumerable<AuditLogDto>>
            {
                Data = logDtos,
                IsSuccess = true,
                Message = "Audit logs retrieved successfully"
            };
        }
        public async Task LogActionAsync(Guid userId, string action, string entityName, Guid entityId, string details, string role)
        {
            var auditLog = new AuditLog
            {
                Id = userId,
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                Role = role.ToString(),
                Timestamp = DateTime.UtcNow
            };
            await _auditLogRepository.AddAuditLogAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Audit log created: {Action} on {EntityName} by User {UserId}", action, entityName, userId);
        }
    }
}