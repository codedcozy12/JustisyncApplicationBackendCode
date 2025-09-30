using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string MiddleName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public Role Role { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public Accountstatus Accountstatus { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
    public class UserCreateRequestDto
    {
        public required string Username { get; set; } = default!;
        public required string Email { get; set; } = default!;
        public required string Password { get; set; } = default!;
        public required string FirstName { get; set; } = default!;
        public required string LastName { get; set; } = default!;
        public required string MiddleName { get; set; } = default!;
        public required string PhoneNumber { get; set; } = default!;
        public IFormFile?  ProfilePictureUrl { get; set; }
    }
    public class UserUpdateRequestDto
    {
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? MiddleName { get; set; }
            public string? PhoneNumber { get; set; }
            public IFormFile? ProfilePictureUrl { get; set; }
     }
    public class UserLoginRequestDto
    {
            public string Email { get; set; } = default!;
            public string Password { get; set; } = default!;
    }
    public class UserForgotPasswordRequestDto
    {
        public string Email { get; set; } = default!;
    }
    public class UserResetPasswordRequestDto
    {
        public string Email { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string OldPassword { get; set; } = default!;
    }
}