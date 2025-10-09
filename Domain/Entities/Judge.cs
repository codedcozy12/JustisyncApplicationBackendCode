namespace Domain.Entities
{
    public class Judge : BaseEntities
    {
        public ICollection<Case> Cases { get; set; } = [];
        public ICollection<JudgeCourt> JudgeCourts { get; set; } = [];
        public DateTime? AppointmentDate { get; set; }
        public User User { get; set; } = default!;
        public Guid UserId { get; set; }
    }
}