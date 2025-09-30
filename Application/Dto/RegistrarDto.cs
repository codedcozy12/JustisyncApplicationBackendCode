using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class RegistrarDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string MiddleName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? ProfilePictureUrl { get; set; }
        public Guid CourtId { get; set; }
        public bool IsActive { get; set; }
        public required string CourtName { get; set; }
        public Guid UserId { get; set; }
    }
    public class RegistrarCreateRequestDto
    {
        public required string Username { get; set; } = default!;
        public required string Email { get; set; } = default!;
        public required string Password { get; set; } = default!;
        public required string FirstName { get; set; } = default!;
        public required string LastName { get; set; } = default!;
        public required string MiddleName { get; set; } = default!;
        public required string PhoneNumber { get; set; } = default!;
        public IFormFile? ProfilePictureUrl { get; set; }
        public required Guid CourtId { get; set; }
    }
    public class RegistrarUpdateRequestDto
    {
        public Guid CourtId { get; set; }
    }
    public class AssignRegistrarRequestDto
    {
        public Guid RegistrarId { get; set; }
        public Guid CourtId { get; set; }
        public DateTime? AppointmentDate { get; set; }
    }
}