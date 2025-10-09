namespace Domain.Entities
{
    public class CourtJurisdiction
    {
        public Guid Id { get; set; }
        public string SubjectMatter { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal? MonetaryLimit { get; set; }
        public Guid CourtId { get; set; }
        public Court Court { get; set; } = default!;
    }
}