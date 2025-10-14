using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dto;
using Application.Interfaces.AuthService;
using Infrastructures.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructures.Service.Auth
{
    public class AuthService(IOptions<JwtSettings> config) : IAuthService
    {
        private readonly JwtSettings _config = config.Value;

        public string GenerateJwtToken(UserDto userDto)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key));

            var claims = new List<Claim>
        {
            new Claim("UserId", userDto.Id.ToString()),
            new Claim("Email", userDto.Email),
            new Claim("Username", userDto.Username),
            new Claim("role", userDto.Role.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes((double)_config.ExpiryTime),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}