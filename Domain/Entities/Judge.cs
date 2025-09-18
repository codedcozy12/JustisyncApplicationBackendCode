namespace Domain.Entities
{
    public class Judge : BaseEntities
    {
        public Guid CourtId { get; set; }
        public Court Court { get; set; }
        public ICollection<Case> Cases { get; set; } = [];
        public DateTime? AppointmentDate { get; set; }
        public User User { get; set; } = default!;
        public Guid UserId { get; set; }
    }
}