using Microsoft.IdentityModel.Tokens;
using PawOfHelp.DTOs.Auth;
using PawOfHelp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PawOfHelp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // Здесь будет сохранение пользователя в БД
            // Пока возвращаем заглушку

            var token = GenerateJwtToken(request.Email, request.Role);

            return new AuthResponseDto
            {
                AccessToken = token,
                Email = request.Email,
                Role = request.Role
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Здесь будет проверка пользователя в БД
            // Пока заглушка: считаем, что логин успешен для test@example.com / Password123!

            if (request.Email == "test@example.com" && request.Password == "Password123!")
            {
                var token = GenerateJwtToken(request.Email, "Volunteer");

                return new AuthResponseDto
                {
                    AccessToken = token,
                    Email = request.Email,
                    Role = "Volunteer"
                };
            }

            throw new Exception("Неверный email или пароль");
        }

        private string GenerateJwtToken(string email, string role)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
