using Domain.Enums;

namespace Application.Dto
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Action { get; set; } = default!;
        public string EntityName { get; set; } = default!;
        public Guid? EntityId { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}