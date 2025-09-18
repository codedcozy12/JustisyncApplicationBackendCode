namespace Domain.Entities
{
    public class Notification : BaseEntities
    {
        public string Message { get; set; } = default!;
        public string Title { get; set; } = default!;
        public bool IsRead { get; set; } = false;
        public User User { get; set; } = default!;
        public Guid ToId { get; set; }
    }
}
