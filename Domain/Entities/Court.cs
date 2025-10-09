using Domain.Enums;

namespace Domain.Entities
{
    public class Court : BaseEntities
    {
            public required string Name { get; set; }
            public required string Type { get; set; }
            public required string Address { get; set; }
            public required string City { get; set; }
            public required string State { get; set; }
            public required string ContactEmail { get; set; }
            public required string ContactPhone { get; set; }
            public string Location { get; set; } = default!;
            public ICollection<Registrar> Registrars { get; set; } = [];
            public ICollection<JudgeCourt> JudgeCourts { get; set; } = [];
            public ICollection<Jurisdiction> Jurisdictions { get; set; } = [];
            public bool IsActive { get; set; } = true;
            public ICollection<CourtSession> CourtSessions { get; set; } = [];
            public ICollection<Case> CourtCases { get; set; } = [];
        }
    }