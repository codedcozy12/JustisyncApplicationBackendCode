using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class JudgeDto
    {
        public Guid Id { get; set; }
        public Guid CourtAssignedId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string MiddleName { get; set; }
        public required string PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string CourtName { get; set; } = default!;
        public DateTime? AppointmentDate { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
    public class JudgeCreateRequestDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; } = default!;
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string MiddleName { get; set; }
        public required string PhoneNumber { get; set; }
        public IFormFile? ProfilePictureUrl { get; set; }
        public Guid CourtAssignedId { get; set; }
        public DateTime? AppointmentDate { get; set; }
    }
    public class JudgeUpdateRequestDto
    {
        public Guid CourtAssignedId { get; set; }
        public DateTime? AppointmentDate { get; set; }
    }
}