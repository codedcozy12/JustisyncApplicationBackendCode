using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuditLogsController> _logger;
        public AuditLogsController(IAuditLogService auditLogService, ILogger<AuditLogsController> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Get all audit logs", Description = "Retrieves a list of all audit logs in the system.")]
        public async Task<IActionResult> GetAllAuditLogs()
        {
            var result = await _auditLogService.GetAllAuditLogsAsync();
            return Ok(result);
        }
        [HttpGet("{userId:guid}")]
        [SwaggerOperation(Summary = "Get audit logs by user ID", Description = "Retrieves audit logs associated with a specific user by their unique identifier.")]
        public async Task<IActionResult> GetLogsByUserAsync(Guid userId)
        {
            var result = await _auditLogService.GetLogsByUserAsync(userId);
            return Ok(result);
        }
        [HttpGet("role/{role}")]
        [SwaggerOperation(Summary = "Get audit logs by role", Description = "Retrieves audit logs associated with users of a specific role.")]
        public async Task<IActionResult> GetLogsByRoleAsync(string role)
        {
            var result = await _auditLogService.GetLogsByRoleAsync(role);
            return Ok(result);
        }
        [HttpGet("entity/{entityName}/{entityId:guid}")]
        [SwaggerOperation(Summary = "Get audit logs by entity", Description = "Retrieves audit logs related to a specific entity by its name and unique identifier.")]
        public async Task<IActionResult> GetLogsByEntityAsync(string entityName, Guid entityId)
        {
            var result = await _auditLogService.GetLogsByEntityAsync(entityName, entityId);
            return Ok(result);
        }
        [HttpGet("dateRange")]
        [SwaggerOperation(Summary = "Get audit logs by date range", Description = "Retrieves audit logs that were created within a specified date range.")]
        public async Task<IActionResult> GetLogsByDateRangeAsync([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _auditLogService.GetLogsByDateRangeAsync(start, end);
            return Ok(result);
        }
        [HttpGet("action/{action}")]
        [SwaggerOperation(Summary = "Get audit logs by action", Description = "Retrieves audit logs associated with a specific action performed in the system.")]
        public async Task<IActionResult> GetLogsByActionAsync(string action)
        {
            var result = await _auditLogService.GetLogsByActionAsync(action);
            return Ok(result);
        }
    }
}