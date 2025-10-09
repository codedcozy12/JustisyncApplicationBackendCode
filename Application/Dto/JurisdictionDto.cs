namespace Application.Dto
{
    public class JurisdictionDto
    {
        public Guid Id { get; set; }
        public string SubjectMatter { get; set; }
        public decimal? MonetaryLimit { get; set; }
        public List<Guid> CourtIds { get; set; }
    }
    public class JurisdictionCreateRequestDto
    {
        public required string SubjectMatter { get; set; }
        public decimal? MonetaryLimit { get; set; }
        public ICollection<Guid>? CourtIds { get; set; }
    }
    public class JurisdictionUpdateRequestDto
    {
        public required Guid Id { get; set; }
        public required string SubjectMatter { get; set; }
        public decimal? MonetaryLimit { get; set; }
        public ICollection<Guid>? CourtIds { get; set; }
    }
}