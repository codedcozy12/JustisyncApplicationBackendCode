namespace Domain.Entities
{
    public class CourtJurisdiction
    {
        public Guid Id { get; set; }
        public string SubjectMatter { get; set; } = string.Empty;
        public decimal? MonetaryLimit { get; set; }
        public ICollection<Court> Courts { get; set; } = [];
    }
}