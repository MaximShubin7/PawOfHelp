// Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Animal;
using PawOfHelp.DTOs.Auth;
using PawOfHelp.DTOs.Comment;
using PawOfHelp.DTOs.Post;
using PawOfHelp.DTOs.Public;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IAnimalService _animalService;

    public AuthService(
        AppDbContext context,
        IEmailService emailService,
        IConfiguration configuration,
        IAnimalService animalService)
    {
        _context = context;
        _emailService = emailService;
        _configuration = configuration;
        _animalService = animalService;
    }

    private async Task<short> GetRoleIdByNameAsync(string roleName)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
            throw new Exception($"Роль '{roleName}' не найдена в базе данных");

        return role.Id;
    }

    private async Task<string> GetRoleNameByIdAsync(short roleId)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == roleId);

        return role?.Name ?? "Неизвестно";
    }

    private async Task<List<string>> GetUserCompetenciesAsync(Guid userId)
    {
        var competencies = await _context.UserCompetencies
            .Where(uc => uc.UserId == userId)
            .Include(uc => uc.Competency)
            .Where(uc => uc.Competency != null)
            .Select(uc => uc.Competency!.Name)
            .ToListAsync();

        return competencies;
    }

    private async Task<List<string>> GetUserPreferencesAsync(Guid userId)
    {
        var preferences = await _context.UserPreferences
            .Where(up => up.UserId == userId)
            .Include(up => up.Preference)
            .Where(up => up.Preference != null)
            .Select(up => up.Preference!.Name)
            .ToListAsync();

        return preferences;
    }

    private async Task<List<string>> GetUserAvailabilitiesAsync(Guid userId)
    {
        var availabilities = await _context.UserAvailabilities
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Availability)
            .Where(ua => ua.Availability != null)
            .Select(ua => ua.Availability!.Name)
            .ToListAsync();

        return availabilities;
    }

    private async Task<List<string>> GetOrganizationConstantNeedsAsync(Guid organizationId)
    {
        var needs = await _context.OrganizationConstantNeeds
            .Where(ocn => ocn.OrganizationId == organizationId)
            .Include(ocn => ocn.ConstantNeed)
            .Where(ocn => ocn.ConstantNeed != null)
            .Select(ocn => ocn.ConstantNeed!.Name)
            .ToListAsync();

        return needs;
    }

    private async Task<string?> GetLocationNameByIdAsync(short? locationId)
    {
        if (!locationId.HasValue)
            return null;

        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == locationId.Value);

        return location?.Name;
    }

    private async Task<List<CommentResponseDto>> GetLatestCommentsAsync(Guid userId, int count = 5)
    {
        var comments = await _context.Comments
            .Where(c => c.RecipientId == userId)
            .Include(c => c.Sender)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Rating = c.Rating,
                Description = c.Description,
                Sender = new PublicProfileDto
                {
                    Id = c.SenderId,
                    Name = c.Sender != null ? c.Sender.Name : string.Empty
                },
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return comments;
    }

    private async Task<PostResponseDto?> GetLatestPostAsync(Guid organizationId)
    {
        var post = await _context.Posts
            .Include(p => p.Photos)
            .Where(p => p.OrganizationId == organizationId)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();

        if (post == null)
            return null;

        return new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            PhotoUrls = post.Photos.Select(ph => ph.PhotoUrl).ToList(),
            CreatedAt = post.CreatedAt
        };
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
            new Claim(ClaimTypes.Role, user.RoleId.ToString())
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
        var roleId = await GetRoleIdByNameAsync(request.Role);

        var verificationCode = new VerificationCode
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = passwordHash,
            Name = string.Empty,
            Role = roleId,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            AttemptsLeft = 3,
            CreatedAt = DateTime.UtcNow
        };

        _context.VerificationCodes.Add(verificationCode);
        await _context.SaveChangesAsync();
        await _emailService.SendVerificationCodeAsync(request.Email, code);
    }

    public async Task<AuthResponseDto> ConfirmEmailAsync(ConfirmEmailRequestDto request)
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
            LocationId = null,
            RoleId = verification.Role,
            Age = null,
            Description = null,
            SumRating = 0,
            CountRating = 0,
            CountTasks = 0,
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
            _context.OrganizationDetails.Add(organizationDetails);
        }

        _context.VerificationCodes.Remove(verification);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        var locationName = await GetLocationNameByIdAsync(user.LocationId);
        var latestComments = await GetLatestCommentsAsync(user.Id, 5);
        var latestAnimals = await _animalService.GetLatestUserAnimalsAsync(user.Id);
        var roleName = await GetRoleNameByIdAsync(user.RoleId);

        if (user.RoleId == 2)
        {
            var orgDetails = await _context.OrganizationDetails
                .FirstOrDefaultAsync(o => o.UserId == user.Id);

            var latestPost = await GetLatestPostAsync(user.Id);
            var constantNeeds = await GetOrganizationConstantNeedsAsync(user.Id);

            return new OrganizationAuthResponseDto
            {
                AccessToken = token,
                TokenType = "Bearer",
                UserId = user.Id,
                Name = user.Name,
                Role = roleName,
                Age = user.Age,
                Description = user.Description,
                PhotoUrl = user.PhotoUrl,
                Location = locationName,
                CountTasks = user.CountTasks,
                LatestComments = latestComments,
                LatestAnimals = latestAnimals,
                SumRating = user.SumRating,
                CountRating = user.CountRating,
                CreatedAt = user.CreatedAt,
                Phone = orgDetails?.Phone,
                Website = orgDetails?.Website,
                DonationDetails = orgDetails?.DonationDetails,
                LatestPost = latestPost,
                ConstantNeeds = constantNeeds
            };
        }

        return new VolunteerAuthResponseDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            UserId = user.Id,
            Name = user.Name,
            Role = roleName,
            Age = user.Age,
            Description = user.Description,
            PhotoUrl = user.PhotoUrl,
            Location = locationName,
            CountTasks = user.CountTasks,
            LatestComments = latestComments,
            LatestAnimals = latestAnimals,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            CreatedAt = user.CreatedAt,
            Competencies = new List<string>(),
            Preferences = new List<string>(),
            Availabilities = new List<string>()
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.Location)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            throw new Exception("Неверный email или пароль");

        var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!isValidPassword)
            throw new Exception("Неверный email или пароль");

        var token = GenerateJwtToken(user);
        var locationName = await GetLocationNameByIdAsync(user.LocationId);
        var latestComments = await GetLatestCommentsAsync(user.Id, 5);
        var latestAnimals = await _animalService.GetLatestUserAnimalsAsync(user.Id);
        var roleName = await GetRoleNameByIdAsync(user.RoleId);

        if (user.RoleId == 2)
        {
            var orgDetails = await _context.OrganizationDetails
                .FirstOrDefaultAsync(o => o.UserId == user.Id);

            var latestPost = await GetLatestPostAsync(user.Id);
            var constantNeeds = await GetOrganizationConstantNeedsAsync(user.Id);

            return new OrganizationAuthResponseDto
            {
                AccessToken = token,
                TokenType = "Bearer",
                UserId = user.Id,
                Name = user.Name,
                Role = roleName,
                Age = user.Age,
                Description = user.Description,
                PhotoUrl = user.PhotoUrl,
                Location = locationName,
                CountTasks = user.CountTasks,
                LatestComments = latestComments,
                LatestAnimals = latestAnimals,
                SumRating = user.SumRating,
                CountRating = user.CountRating,
                CreatedAt = user.CreatedAt,
                Phone = orgDetails?.Phone,
                Website = orgDetails?.Website,
                DonationDetails = orgDetails?.DonationDetails,
                LatestPost = latestPost,
                ConstantNeeds = constantNeeds
            };
        }

        var competencies = await GetUserCompetenciesAsync(user.Id);
        var preferences = await GetUserPreferencesAsync(user.Id);
        var availabilities = await GetUserAvailabilitiesAsync(user.Id);

        return new VolunteerAuthResponseDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            UserId = user.Id,
            Name = user.Name,
            Role = roleName,
            Age = user.Age,
            Description = user.Description,
            PhotoUrl = user.PhotoUrl,
            Location = locationName,
            CountTasks = user.CountTasks,
            LatestComments = latestComments,
            LatestAnimals = latestAnimals,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            CreatedAt = user.CreatedAt,
            Competencies = competencies,
            Preferences = preferences,
            Availabilities = availabilities
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