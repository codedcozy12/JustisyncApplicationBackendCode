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
        public IFormFile? ProfilePictureUrl { get; set; }
        public Guid CourtId { get; set; }
        public required string CourtName { get; set; }
        public Guid UserId { get; set; }
    }
    public class RegistrarCreateRequestDto
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string MiddleName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public IFormFile? ProfilePictureUrl { get; set; }
        public Guid CourtId { get; set; }
    }
    public class RegistrarUpdateRequestDto
    {
        public Guid CourtId { get; set; }
    }
}