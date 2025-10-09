using Domain.Enums;

namespace Application.Dto
{
   public class CasePartyDto
    {
            public Guid Id { get; set; }
            public Guid CaseId { get; set; }
            public string FullName { get; set; } = default!;
            public string Address { get; set; } = default!;
            public string Contact { get; set; } = default!;
            public PartyRole Role { get; set; }
            public string? Email { get; set; }
            public DateTime DateCreated { get; set; }
        }

    }
    public class CasePartyCreateDto
    {
        public Guid? CaseId { get; set; }
        public string FullName { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Contact { get; set; } = default!;
        public PartyRole Role { get; set; }
        public string? Email { get; set; }
    }
    public class CasePartyUpdateDto
    {
        public Guid Id { get; set; } 
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Contact { get; set; }
        public PartyRole? Role { get; set; }
        public string? Email { get; set; }
    }