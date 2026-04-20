// Services/OrganizationService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Organization;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class OrganizationService : IOrganizationService
{
    private readonly AppDbContext _context;
    private readonly IImageKitService _imageKitService;

    public OrganizationService(AppDbContext context, IImageKitService imageKitService)
    {
        _context = context;
        _imageKitService = imageKitService;
    }

    public async Task<OrganizationResponseDto> GetOrganizationAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.OrganizationDetails)
            .FirstOrDefaultAsync(u => u.Id == userId && u.Role == 2);

        if (user == null)
            throw new Exception("Организация не найдена");

        return new OrganizationResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Phone = user.OrganizationDetails?.Phone,
            Website = user.OrganizationDetails?.Website,
            DonationDetails = user.OrganizationDetails?.DonationDetails,
            Description = user.Description,
            PhotoUrl = user.PhotoUrl,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<OrganizationResponseDto> UpdateOrganizationAsync(Guid userId, UpdateOrganizationDto dto)
    {
        var user = await _context.Users
            .Include(u => u.OrganizationDetails)
            .FirstOrDefaultAsync(u => u.Id == userId && u.Role == 2);

        if (user == null)
            throw new Exception("Организация не найдена");

        if (dto.Name != null)
            user.Name = dto.Name;

        if (dto.Description != null)
            user.Description = dto.Description;

        if (user.OrganizationDetails != null)
        {
            if (dto.Phone != null)
                user.OrganizationDetails.Phone = dto.Phone;
            if (dto.Website != null)
                user.OrganizationDetails.Website = dto.Website;
            if (dto.DonationDetails != null)
                user.OrganizationDetails.DonationDetails = dto.DonationDetails;
        }

        if (dto.Photo != null)
        {
            var oldPhotoUrl = user.PhotoUrl;
            var photoUrl = await _imageKitService.UploadImageAsync(dto.Photo, userId.ToString());
            if (photoUrl != null)
                user.PhotoUrl = photoUrl;

            if (!string.IsNullOrEmpty(oldPhotoUrl) && user.PhotoUrl != oldPhotoUrl)
            {
                var fileId = await _imageKitService.GetFileIdFromUrlAsync(oldPhotoUrl);
                if (fileId != null)
                    await _imageKitService.DeleteImageAsync(fileId);
            }
        }
        else if (!string.IsNullOrEmpty(dto.PhotoUrlFromWeb))
        {
            var oldPhotoUrl = user.PhotoUrl;
            var photoUrl = await _imageKitService.UploadImageFromUrlAsync(dto.PhotoUrlFromWeb, userId.ToString());
            if (photoUrl != null)
                user.PhotoUrl = photoUrl;

            if (!string.IsNullOrEmpty(oldPhotoUrl) && user.PhotoUrl != oldPhotoUrl)
            {
                var fileId = await _imageKitService.GetFileIdFromUrlAsync(oldPhotoUrl);
                if (fileId != null)
                    await _imageKitService.DeleteImageAsync(fileId);
            }
        }

        await _context.SaveChangesAsync();

        return new OrganizationResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Phone = user.OrganizationDetails?.Phone,
            Website = user.OrganizationDetails?.Website,
            DonationDetails = user.OrganizationDetails?.DonationDetails,
            Description = user.Description,
            PhotoUrl = user.PhotoUrl,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}