using Domain.Enums;

namespace Domain.Entities
{
    public class CourtSession : BaseEntities
    {
        public Guid CaseId { get; set; }  
        public Case Case { get; set; } = default!;
        public Guid CourtId { get; set; }
        public Court Court { get; set; } = default!;
        public Guid JudgeId { get; set; }
        public User Judge { get; set; } = default!;
        public DateTime ScheduledDate { get; set; }
        public DateTime? StartTime { get; set; }         
        public DateTime? EndTime { get; set; }
        public CourtSessionStatus Status { get; set; }
        public ICollection<User> Users { get; set; } = [];
        public ICollection<CaseProceedingRecord> Proceedings { get; set; } = [];
        public string? VirtualMeetingUrl { get; set; }
        public string VirtualMeetingRoom { get; set; } = default;
        public string MeetingAccessToken { get; set; } = default;
        public DateTime MeetingJoinTime { get; set; }
        public DateTime MeetingEndTime { get; set; }
    }
}