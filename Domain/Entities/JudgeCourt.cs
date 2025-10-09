namespace Domain.Entities
{
    public class JudgeCourt
    {
        public Guid JudgeId { get; set; }
        public Judge Judge { get; set; } = default!;
        public Guid CourtId { get; set; }
        public Court Court { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
    }
}
