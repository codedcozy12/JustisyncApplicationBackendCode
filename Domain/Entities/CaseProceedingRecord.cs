using Domain.Enums;

namespace Domain.Entities
{
    public class CaseProceedingRecord : BaseEntities
    {
        public Guid CaseId { get; set; }        
        public Guid CourtSessionId { get; set; }            
        public ProceedingType Type { get; set; }          
        public required string Title { get; set; }
        public required string Description { get; set; }           
        public ProceedingStatus Status { get; set; }         
        public DateTime ScheduledDate { get; set; }          
        public DateTime? CompletedDate { get; set; }        
        public required string Outcome { get; set; }  
        public string RecordingLink { get; set; } = default!;
        public string SessionLink { get; set; } = default!;
        public Guid RecordedByUserId { get; set; }         
        public required Case Case { get; set; }
        public CourtSessionStatus CourtSession { get; set; }
    }
}