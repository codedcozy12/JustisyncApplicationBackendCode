using Domain.Enums;

namespace Application.Dto
{
    public class CourtSessionDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public string? CaseNumber { get; set; }
        public Guid CourtId { get; set; }
        public string? Court { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public CourtSessionStatus Status { get; set; }
        public List<CaseProceedingRecordDto>? Proceedings { get; set; }
    }
    public class CourtSessionCreateRequestDto
    {
        public Guid CaseId { get; set; }
        public Guid CourtId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public CourtSessionStatus Status { get; set; }
        public List<CaseProceedingRecordCreateDto>? Proceedings { get; set; }
    }
    public class CourtSessionUpdateRequestDto
    {
        public Guid Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public CourtSessionStatus Status { get; set; }
    }
}