using Domain.Enums;

namespace Application.Dto
{
    public class CaseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default;
        public string CaseNumber { get; set; } = default!;
        public Guid ClientId { get; set; }
        public Guid LawyerId { get; set; }
        public Guid? CourtId { get; set; }
        public Guid? JudgeId { get; set; }
        public OffenseType OffenseType { get; set; }
        public string OffenseLocation { get; set; } = default!;
        public string Description { get; set; } = default!;
        public CaseStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CasePartyDto>? Parties { get; set; }
        public List<Guid>? DocumentIds { get; set; }
        public List<CaseProceedingRecordDto>? Proceedings { get; set; }
    }
    public class CaseCreateRequestDto
    {
        public string CaseNumber { get; set; } = default!;
        public required string Title { get; set; }
        public Guid ClientId { get; set; }
        public Guid LawyerId { get; set; }
        //public Guid CourtId { get; set; }
        //public Guid JudgeId { get; set; }
        public OffenseType OffenseType { get; set; }
        public string OffenseLocation { get; set; } = default!;
        public string Description { get; set; } = default!;
        public CaseStatus Status { get; set; }
        public List<CasePartyCreateDto>? Parties { get; set; }
    }
    public class CaseUpdateRequestDto
    {
        public Guid Id { get; set; }
        public string? CaseNumber { get; set; }
        public Guid? LawyerId { get; set; }
        public Guid? JudgeId { get; set; }
        public OffenseType? OffenseType { get; set; }
        public string? Description { get; set; }
        public CaseStatus? Status { get; set; }
        public List<Guid>? DocumentIds { get; set; }
    }
    public class CaseUpdateVerificationRequestDto
    {
        public Guid CaseId { get; set; }
        public CaseApprovalStatus ApprovalStatus { get; set; }
        public string RejectionReason { get; set; } = default!;
    }
}