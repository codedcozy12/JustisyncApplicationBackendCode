using Domain.Enums;

namespace Domain.Entities
{
    public class Lawyer : BaseEntities
    {
        public User User { get; set; } = default!;
        public Guid UserId { get; set; }
        public string? LawSchoolName { get; set; }
        public required string UniversityName { get; set; }
        public required string BarMembershipNumber { get; set; }
        public ICollection<Certificate> Certificates { get; set; } = [];
        public LawyerSpecialization LawyerSpecialization { get; set; }
        public lawyerVerificationStatus VerificationStatus { get; set; }
    }
}