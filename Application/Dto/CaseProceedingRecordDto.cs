namespace Application.Dto
{
    public class CaseProceedingRecordDto
    {
            public Guid Id { get; set; }
            public Guid CaseId { get; set; }
            public Guid JudgeId { get; set; }
            public Guid? CourtSessionId { get; set; }
            public string? Notes { get; set; }
            public DateTime ProceedingDate { get; set; }
            public string Description { get; set; } = default!;
            public string? Outcome { get; set; }
        }
    public class CaseProceedingRecordCreateDto
    {
        public Guid CaseId { get; set; }
        public Guid JudgeId { get; set; }
        public Guid CourtSessionId { get; set; }
        public string? Notes { get; set; }
        public DateTime ProceedingDate { get; set; }
        public string Description { get; set; } = default!;
        public string? Outcome { get; set; }
    }
    public class CaseProceedingRecordUpdateDto
    {
        public Guid Id { get; set; }
        public Guid JudgeId { get; set; }
        public string? Notes { get; set; }
        public Guid? CourtSessionId { get; set; }
        public DateTime? ProceedingDate { get; set; }
        public string? Description { get; set; }
        public string? Outcome { get; set; }
    }
}