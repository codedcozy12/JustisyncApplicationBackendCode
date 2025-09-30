using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class LawyerDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public required string FirstName { get; set; } = default!;
        public required string LastName { get; set; } = default!;
        public required string MiddleName { get; set; } = default!;
        public required string PhoneNumber { get; set; } = default!;
        public string ProfilePictureUrl { get; set; } = default;
        public string? LawSchoolName { get; set; }
        public string UniversityName { get; set; } = default!;
        public string BarMembershipNumber { get; set; } = default!;
        public List<CertificateDto> Certificates { get; set; } = new();
        public LawyerSpecialization LawyerSpecialization { get; set; }
        public lawyerVerificationStatus VerificationStatus { get; set; }
    }
    public class LawyerCreateRequestDto
    {
        public required string Username { get; set; } = default!;
        public required string Email { get; set; } = default!;
        public required string Password { get; set; } = default!;
        public required string FirstName { get; set; } = default!;
        public required string LastName { get; set; } = default!;
        public required string MiddleName { get; set; } = default!;
        public required string PhoneNumber { get; set; } = default!;
        public IFormFile? ProfilePictureUrl { get; set; }
        public required string LawSchoolName { get; set; }
        public string UniversityName { get; set; } = default!;
        public string BarMembershipNumber { get; set; } = default!;
        public LawyerSpecialization LawyerSpecialization { get; set; }
        public List<string> CertificateTypes { get; set; } = new();
        public List<IFormFile> CertificateFiles { get; set; } = new();
    }
    public class LawyerUpdateRequestDto
    {
        public string? LawSchoolName { get; set; }
        public string UniversityName { get; set; } = default!;
        public string BarMembershipNumber { get; set; } = default!;
        public lawyerVerificationStatus VerificationStatus { get; set; }
        public List<CertificateUpdateRequestDto> Certificates { get; set; } = new();
        public LawyerSpecialization LawyerSpecialization { get; set; }
    }
    public class LawyerUpdateVerifcationRequestDto
    {
        public Guid LwayerId { get; set; }
        public lawyerVerificationStatus VerificationStatus { get; set; }
    }
}