// Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Auth;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IEmailService emailService, IConfiguration configuration)
    {
        _context = context;
        _emailService = emailService;
        _configuration = configuration;
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["Jwt:SecretKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())
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

    public async Task RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
            throw new Exception("Пользователь с таким email уже зарегистрирован");

        var existingCode = await _context.VerificationCodes.FirstOrDefaultAsync(c => c.Email == request.Email);
        if (existingCode != null)
            _context.VerificationCodes.Remove(existingCode);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var code = new Random().Next(100000, 999999).ToString();

        var verificationCode = new VerificationCode
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = passwordHash,
            Name = string.Empty,
            Role = request.Role,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            AttemptsLeft = 3,
            CreatedAt = DateTime.UtcNow
        };

        _context.VerificationCodes.Add(verificationCode);
        await _context.SaveChangesAsync();
        await _emailService.SendVerificationCodeAsync(request.Email, code);
    }

    public async Task<VolunteerAuthResponseDto> ConfirmEmailAsync(ConfirmEmailRequestDto request)
    {
        var verification = await _context.VerificationCodes
            .FirstOrDefaultAsync(c => c.Email == request.Email);

        if (verification == null)
            throw new Exception("Код не найден. Запросите новый код");

        if (verification.ExpiresAt < DateTime.UtcNow)
            throw new Exception("Код истёк. Запросите новый");

        if (verification.AttemptsLeft <= 0)
            throw new Exception("Превышено число попыток. Запросите новый код");

        if (verification.Code != request.Code)
        {
            verification.AttemptsLeft--;
            await _context.SaveChangesAsync();
            throw new Exception($"Неверный код. Осталось попыток: {verification.AttemptsLeft}");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = verification.Email,
            Password = verification.Password,
            Name = verification.Name,
            Role = verification.Role,
            Age = null,
            Description = null,
            SumRating = 0,
            CountRating = 0,
            CreatedAt = DateTime.UtcNow,
            PhotoUrl = null
        };

        _context.Users.Add(user);

        if (verification.Role == 2)
        {
            var organizationDetails = new OrganizationDetails
            {
                UserId = user.Id
            };
            _context.OrganizationsDetails.Add(organizationDetails);
        }

        _context.VerificationCodes.Remove(verification);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        if (user.Role == 2 && user.OrganizationDetails != null)
        {
            return new OrganizationAuthResponseDto
            {
                AccessToken = token,
                TokenType = "Bearer",
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Age = user.Age,
                Description = user.Description,
                PhotoUrl = user.PhotoUrl,
                SumRating = user.SumRating,
                CountRating = user.CountRating,
                CreatedAt = user.CreatedAt,
                Phone = user.OrganizationDetails.Phone,
                Website = user.OrganizationDetails.Website,
                DonationDetails = user.OrganizationDetails.DonationDetails
            };
        }

        return new VolunteerAuthResponseDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            Age = user.Age,
            Description = user.Description,
            PhotoUrl = user.PhotoUrl,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<VolunteerAuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.OrganizationDetails)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            throw new Exception("Неверный email или пароль");

        var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!isValidPassword)
            throw new Exception("Неверный email или пароль");

        var token = GenerateJwtToken(user);

        if (user.Role == 2 && user.OrganizationDetails != null)
        {
            return new OrganizationAuthResponseDto
            {
                AccessToken = token,
                TokenType = "Bearer",
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Age = user.Age,
                Description = user.Description,
                PhotoUrl = user.PhotoUrl,
                SumRating = user.SumRating,
                CountRating = user.CountRating,
                CreatedAt = user.CreatedAt,
                Phone = user.OrganizationDetails.Phone,
                Website = user.OrganizationDetails.Website,
                DonationDetails = user.OrganizationDetails.DonationDetails
            };
        }

        return new VolunteerAuthResponseDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            Age = user.Age,
            Description = user.Description,
            PhotoUrl = user.PhotoUrl,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return;

        var existingCode = await _context.VerificationCodes.FirstOrDefaultAsync(c => c.Email == request.Email);
        if (existingCode != null) _context.VerificationCodes.Remove(existingCode);

        var code = new Random().Next(100000, 999999).ToString();

        var resetCode = new VerificationCode
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            AttemptsLeft = 3,
            CreatedAt = DateTime.UtcNow
        };

        _context.VerificationCodes.Add(resetCode);
        await _context.SaveChangesAsync();
        await _emailService.SendPasswordResetCodeAsync(request.Email, code);
    }

    public async Task ResetPasswordWithCodeAsync(ResetPasswordWithCodeRequestDto request)
    {
        if (request.NewPassword.Length < 8)
            throw new Exception("Пароль должен содержать минимум 8 символов");

        var resetCode = await _context.VerificationCodes.FirstOrDefaultAsync(c => c.Email == request.Email);
        if (resetCode == null) throw new Exception("Код не найден. Запросите новый код");
        if (resetCode.ExpiresAt < DateTime.UtcNow) throw new Exception("Код истёк. Запросите новый");
        if (resetCode.AttemptsLeft <= 0) throw new Exception("Превышено число попыток. Запросите новый код");

        if (resetCode.Code != request.Code)
        {
            resetCode.AttemptsLeft--;
            await _context.SaveChangesAsync();
            throw new Exception($"Неверный код. Осталось попыток: {resetCode.AttemptsLeft}");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) throw new Exception("Пользователь не найден");

        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.Password = newPasswordHash;

        _context.VerificationCodes.Remove(resetCode);
        await _context.SaveChangesAsync();
    }
}