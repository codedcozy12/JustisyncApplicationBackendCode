namespace Domain.Entities
{
    public class Document : BaseEntities
    {
        public Case Case { get; set; } = default!;
        public required Guid CaseId { get; set; }
        public required string FileName { get; set; }
        public required string FileType { get; set; }
        public required string FileUrl { get; set; }
    }
}
