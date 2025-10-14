using Domain.Enums;

namespace Domain.Entities
{
    public class CaseParty
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CaseId { get; set; }
        public PartyRole Role { get; set; }
        public Guid? UserId { get; set; }
        public User User { get; set; }
        public Case Case { get; set; }
        public string? FullName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
    }
}