using Domain.Entities;
using Domain.Enums;

namespace Application.Dto
{
    public class CourtDto
    {
            public Guid Id { get; set; }
            public string Name { get; set; } = default!;
            public string Type { get; set; } = default!;
            public string Address { get; set; } = default!;
            public string City { get; set; } = default!;
            public string State { get; set; } = default!;
            public string ContactEmail { get; set; } = default!;
            public string ContactPhone { get; set; } = default!;
            public CourtJurisdiction Jurisdiction { get; set; }
            public List<RegistrarDto> Registrars { get; set; } = new();
            public List<JudgeDto> Judges { get; set; } = new();
            //public List<CourtSessionDto> CourtSessions { get; set; } = new();
            public bool IsActive { get; set; }
        }
    public class CourtCreateRequestDto
    {
            public required string Name { get; set; } = default!;
            public required string Type { get; set; } = default!;
            public required string Address { get; set; } = default!;
            public required string City { get; set; } = default!;
            public required string State { get; set; } = default!;
            public required string ContactEmail { get; set; } = default!;
            public required string ContactPhone { get; set; } = default!;
            public required CourtJurisdiction Jurisdiction { get; set; }
    }
    public class CourtUpdateRequestDto
    {
            public required string ContactEmail { get; set; }
            public required string ContactPhone { get; set; }
            public required CourtJurisdiction Jurisdiction { get; set; }
            public bool IsActive { get; set; }
    }
}