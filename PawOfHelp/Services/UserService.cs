// Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Animal;
using PawOfHelp.DTOs.Comment;
using PawOfHelp.DTOs.Public;
using PawOfHelp.DTOs.User;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IImageKitService _imageKitService;
    private readonly IAnimalService _animalService;

    public UserService(AppDbContext context, IImageKitService imageKitService, IAnimalService animalService)
    {
        _context = context;
        _imageKitService = imageKitService;
        _animalService = animalService;
    }

    private async Task<string?> GetLocationNameByIdAsync(short? locationId)
    {
        if (!locationId.HasValue)
            return null;

        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == locationId.Value);

        return location?.Name;
    }

    private async Task<short?> GetLocationIdByNameAsync(string? locationName)
    {
        if (string.IsNullOrEmpty(locationName))
            return null;

        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Name == locationName);

        if (location == null)
            throw new Exception($"Локация '{locationName}' не найдена");

        return location.Id;
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

    private async Task UpdateUserCompetenciesAsync(Guid userId, List<string>? competencyNames)
    {
        if (competencyNames == null)
            return;

        var competencyIds = new List<short>();

        foreach (var competencyName in competencyNames.Distinct())
        {
            var competency = await _context.Competencies
                .FirstOrDefaultAsync(c => c.Name == competencyName);

            if (competency == null)
                throw new Exception($"Компетенция '{competencyName}' не найдена");

            competencyIds.Add(competency.Id);
        }

        var existingCompetencies = await _context.UserCompetencies
            .Where(uc => uc.UserId == userId)
            .ToListAsync();

        _context.UserCompetencies.RemoveRange(existingCompetencies);

        foreach (var competencyId in competencyIds)
        {
            _context.UserCompetencies.Add(new UserCompetency
            {
                UserId = userId,
                CompetencyId = competencyId
            });
        }
    }

    private async Task UpdateUserPreferencesAsync(Guid userId, List<string>? preferenceNames)
    {
        if (preferenceNames == null)
            return;

        var preferenceIds = new List<short>();

        foreach (var preferenceName in preferenceNames.Distinct())
        {
            var preference = await _context.Preferences
                .FirstOrDefaultAsync(p => p.Name == preferenceName);

            if (preference == null)
                throw new Exception($"Предпочтение '{preferenceName}' не найдено");

            preferenceIds.Add(preference.Id);
        }

        var existingPreferences = await _context.UserPreferences
            .Where(up => up.UserId == userId)
            .ToListAsync();

        _context.UserPreferences.RemoveRange(existingPreferences);

        foreach (var preferenceId in preferenceIds)
        {
            _context.UserPreferences.Add(new UserPreference
            {
                UserId = userId,
                PreferenceId = preferenceId
            });
        }
    }

    private async Task UpdateUserAvailabilitiesAsync(Guid userId, List<string>? availabilityNames)
    {
        if (availabilityNames == null)
            return;

        var availabilityIds = new List<short>();

        foreach (var availabilityName in availabilityNames.Distinct())
        {
            var availability = await _context.Availabilities
                .FirstOrDefaultAsync(a => a.Name == availabilityName);

            if (availability == null)
                throw new Exception($"Доступность '{availabilityName}' не найдена");

            availabilityIds.Add(availability.Id);
        }

        var existingAvailabilities = await _context.UserAvailabilities
            .Where(ua => ua.UserId == userId)
            .ToListAsync();

        _context.UserAvailabilities.RemoveRange(existingAvailabilities);

        foreach (var availabilityId in availabilityIds)
        {
            _context.UserAvailabilities.Add(new UserAvailability
            {
                UserId = userId,
                AvailabilityId = availabilityId
            });
        }
    }

    public async Task<UserProfileResponseDto> GetProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Пользователь не найден");

        var competencies = await GetUserCompetenciesAsync(userId);
        var preferences = await GetUserPreferencesAsync(userId);
        var availabilities = await GetUserAvailabilitiesAsync(userId);
        var latestComments = await GetLatestCommentsAsync(userId, 5);
        var locationName = await GetLocationNameByIdAsync(user.LocationId);
        var latestAnimals = await _animalService.GetLatestUserAnimalsAsync(userId);
        var roleName = await GetRoleNameByIdAsync(user.RoleId);

        return new UserProfileResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = roleName,
            Age = user.Age,
            Description = user.Description,
            Location = locationName,
            PhotoUrl = user.PhotoUrl,
            CountTasks = user.CountTasks,
            Competencies = competencies,
            Preferences = preferences,
            Availabilities = availabilities,
            LatestComments = latestComments,
            LatestAnimals = latestAnimals,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Пользователь не найден");

        if (dto.Name != null)
            user.Name = dto.Name;

        if (dto.Age.HasValue)
            user.Age = dto.Age;

        if (dto.Description != null)
            user.Description = dto.Description;

        if (dto.Location != null)
        {
            var locationId = await GetLocationIdByNameAsync(dto.Location);
            user.LocationId = locationId;
        }

        if (dto.CompetencyNames != null)
        {
            await UpdateUserCompetenciesAsync(userId, dto.CompetencyNames);
        }

        if (dto.PreferenceNames != null)
        {
            await UpdateUserPreferencesAsync(userId, dto.PreferenceNames);
        }

        if (dto.AvailabilityNames != null)
        {
            await UpdateUserAvailabilitiesAsync(userId, dto.AvailabilityNames);
        }

        if (dto.Photo != null)
        {
            var photoUrl = await _imageKitService.UploadImageAsync(dto.Photo, "users", userId.ToString());
            if (photoUrl != null)
                user.PhotoUrl = photoUrl;
        }
        else if (!string.IsNullOrEmpty(dto.PhotoUrlFromWeb))
        {
            var photoUrl = await _imageKitService.UploadImageFromUrlAsync(dto.PhotoUrlFromWeb, "users", userId.ToString());
            if (photoUrl != null)
                user.PhotoUrl = photoUrl;
        }

        await _context.SaveChangesAsync();

        var competencies = await GetUserCompetenciesAsync(userId);
        var preferences = await GetUserPreferencesAsync(userId);
        var availabilities = await GetUserAvailabilitiesAsync(userId);
        var latestComments = await GetLatestCommentsAsync(userId, 5);
        var locationName = await GetLocationNameByIdAsync(user.LocationId);
        var latestAnimals = await _animalService.GetLatestUserAnimalsAsync(userId);
        var roleName = await GetRoleNameByIdAsync(user.RoleId);

        return new UserProfileResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = roleName,
            Age = user.Age,
            Description = user.Description,
            Location = locationName,
            PhotoUrl = user.PhotoUrl,
            Competencies = competencies,
            Preferences = preferences,
            Availabilities = availabilities,
            LatestComments = latestComments,
            LatestAnimals = latestAnimals,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task DeleteProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Пользователь не найден");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Пользователь не найден");

        bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password);

        if (!isOldPasswordValid)
            throw new Exception("Неверный старый пароль");

        if (dto.OldPassword == dto.NewPassword)
            throw new Exception("Новый пароль должен отличаться от старого");

        if (dto.NewPassword.Length < 8)
            throw new Exception("Новый пароль должен содержать минимум 8 символов");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.Password = passwordHash;

        await _context.SaveChangesAsync();
    }

    public async Task<UserProfileResponseDto> GetPublicUserProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Пользователь не найден");

        var competencies = await GetUserCompetenciesAsync(userId);
        var preferences = await GetUserPreferencesAsync(userId);
        var availabilities = await GetUserAvailabilitiesAsync(userId);
        var latestComments = await GetLatestCommentsAsync(userId, 5);
        var locationName = await GetLocationNameByIdAsync(user.LocationId);
        var latestAnimals = await _animalService.GetLatestUserAnimalsAsync(userId);
        var roleName = await GetRoleNameByIdAsync(user.RoleId);

        return new UserProfileResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = roleName,
            Age = user.Age,
            Description = user.Description,
            Location = locationName,
            PhotoUrl = user.PhotoUrl,
            CountTasks = user.CountTasks,
            Competencies = competencies,
            Preferences = preferences,
            Availabilities = availabilities,
            LatestComments = latestComments,
            LatestAnimals = latestAnimals,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<string> GetUserRoleAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Пользователь не найден");

        return await GetRoleNameByIdAsync(user.RoleId);
    }
}