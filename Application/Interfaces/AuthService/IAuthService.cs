using Application.Dto;

namespace Application.Interfaces.AuthService
{
    public interface IAuthService
    {
        string GenerateJwtToken(UserDto userDto);
    }
}