// Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.User;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IImageKitService _imageKitService;

    public UserService(AppDbContext context, IImageKitService imageKitService)
    {
        _context = context;
        _imageKitService = imageKitService;
    }

    public async Task<UserProfileResponseDto> GetProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Пользователь не найден");

        return new UserProfileResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Age = user.Age,
            Description = user.Description,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            Role = user.Role,
            PhotoUrl = user.PhotoUrl
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

        if (dto.Photo != null || !string.IsNullOrEmpty(dto.PhotoUrlFromWeb))
        {
            var oldPhotoUrl = user.PhotoUrl;

            if (dto.Photo != null)
            {
                var photoUrl = await _imageKitService.UploadImageAsync(dto.Photo, userId.ToString());
                if (photoUrl != null)
                    user.PhotoUrl = photoUrl;
            }
            else if (!string.IsNullOrEmpty(dto.PhotoUrlFromWeb))
            {
                var photoUrl = await _imageKitService.UploadImageFromUrlAsync(dto.PhotoUrlFromWeb, userId.ToString());
                if (photoUrl != null)
                    user.PhotoUrl = photoUrl;
            }

            if (!string.IsNullOrEmpty(oldPhotoUrl) && user.PhotoUrl != oldPhotoUrl)
            {
                var fileId = await _imageKitService.GetFileIdFromUrlAsync(oldPhotoUrl);
                if (fileId != null)
                    await _imageKitService.DeleteImageAsync(fileId);
            }
        }

        await _context.SaveChangesAsync();

        return new UserProfileResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Age = user.Age,
            Description = user.Description,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            Role = user.Role,
            PhotoUrl = user.PhotoUrl
        };
    }

    public async Task DeleteProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Пользователь не найден");

        if (!string.IsNullOrEmpty(user.PhotoUrl))
        {
            var fileId = await _imageKitService.GetFileIdFromUrlAsync(user.PhotoUrl);
            if (fileId != null)
                await _imageKitService.DeleteImageAsync(fileId);
        }

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
}