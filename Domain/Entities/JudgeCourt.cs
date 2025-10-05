namespace Domain.Entities
{
    public class JudgeCourt
    {
        public Guid JudgeId { get; set; }
        public Judge Judge { get; set; }
        public Guid CourtId { get; set; }
        public Court Court { get; set; }
    }
}
