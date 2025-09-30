namespace Application.Dto
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public bool IsRead { get; set; }
        public Guid ToId { get; set; }
        public string? UserName { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class NotificationCreateRequestDto
    {
        public required string Title { get; set; } = default!;
        public required string Message { get; set; } = default!;
        public required Guid ToId { get; set; }
    }
    public class NotificationUpdateRequestDto
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
        public required bool IsRead { get; set; }
    }
    public class  ManyNotificationRequestDto
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
    }
}