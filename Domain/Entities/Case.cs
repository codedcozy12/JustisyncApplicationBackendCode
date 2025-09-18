using Domain.Enums;

namespace Domain.Entities
{
    public class Case : BaseEntities
    {                       
        public required string CaseNumber { get; set; }
        public required string Title { get; set; }                    
        public required string Description { get; set; }              
        public required OffenseType OffenseType { get; set; }
        public CaseStatus Status { get; set; } = CaseStatus.Initiated;                   
        public required Guid ClientId { get; set; }
        public User Client { get; set; } = default!;
        public required Guid LawyerId { get; set; }
        public Lawyer Lawyer { get; set; } = default!;
        public Guid JudgeId { get; set; }
        public Judge Judge { get; set; } = default!;
        public Guid RegistrarId { get; set; }
        public Registrar Registrar { get; set; } = default!;
        public ICollection<Document> Documents { get; set; } = [];
        public ICollection<CaseProceedingRecord> Proceedings { get; set; } = [];
        public ICollection<CourtSession> CourtSessions { get; set; } = [];
        public string? AiAnalysisSummary { get; set; }
        public string? PrecedentComparisonResult { get; set; }
    }
}