// Services/OrganizationService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Animal;
using PawOfHelp.DTOs.Comment;
using PawOfHelp.DTOs.Organization;
using PawOfHelp.DTOs.Post;
using PawOfHelp.DTOs.Public;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class OrganizationService : IOrganizationService
{
    private readonly AppDbContext _context;
    private readonly IImageKitService _imageKitService;
    private readonly IPostService _postService;
    private readonly IAnimalService _animalService;

    public OrganizationService(
        AppDbContext context,
        IImageKitService imageKitService,
        IPostService postService,
        IAnimalService animalService)
    {
        _context = context;
        _imageKitService = imageKitService;
        _postService = postService;
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

    private async Task<List<short>> GetConstantNeedIdsByNamesAsync(List<string>? needNames)
    {
        if (needNames == null || !needNames.Any())
            return new List<short>();

        var needIds = new List<short>();

        foreach (var needName in needNames.Distinct())
        {
            var need = await _context.ConstantNeeds
                .FirstOrDefaultAsync(cn => cn.Name == needName);

            if (need == null)
                throw new Exception($"Постоянная потребность '{needName}' не найдена");

            needIds.Add(need.Id);
        }

        return needIds;
    }

    private async Task UpdateOrganizationConstantNeedsAsync(Guid organizationId, List<string>? needNames)
    {
        if (needNames == null)
            return;

        var needIds = await GetConstantNeedIdsByNamesAsync(needNames);

        var existingNeeds = await _context.OrganizationConstantNeeds
            .Where(ocn => ocn.OrganizationId == organizationId)
            .ToListAsync();

        _context.OrganizationConstantNeeds.RemoveRange(existingNeeds);

        foreach (var needId in needIds)
        {
            _context.OrganizationConstantNeeds.Add(new OrganizationConstantNeed
            {
                OrganizationId = organizationId,
                ConstantNeedId = needId
            });
        }
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

    public async Task<OrganizationResponseDto> GetOrganizationAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Location)
            .Include(u => u.OrganizationDetails)
            .FirstOrDefaultAsync(u => u.Id == userId && u.RoleId == 2);

        if (user == null)
            throw new Exception("Организация не найдена");

        var constantNeeds = await GetOrganizationConstantNeedsAsync(userId);
        var latestComments = await GetLatestCommentsAsync(userId, 5);
        var latestPost = await _postService.GetLatestPostByOrganizationAsync(userId);
        var locationName = await GetLocationNameByIdAsync(user.LocationId);
        var latestAnimals = await _animalService.GetLatestUserAnimalsAsync(userId);
        var roleName = await GetRoleNameByIdAsync(user.RoleId);

        return new OrganizationResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = roleName,
            Phone = user.OrganizationDetails?.Phone,
            Website = user.OrganizationDetails?.Website,
            DonationDetails = user.OrganizationDetails?.DonationDetails,
            Description = user.Description,
            Location = locationName,
            PhotoUrl = user.PhotoUrl,
            CountTasks = user.CountTasks,
            ConstantNeeds = constantNeeds,
            LatestComments = latestComments,
            LatestPost = latestPost,
            LatestAnimals = latestAnimals,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<OrganizationResponseDto> UpdateOrganizationAsync(Guid userId, UpdateOrganizationDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Location)
            .Include(u => u.OrganizationDetails)
            .FirstOrDefaultAsync(u => u.Id == userId && u.RoleId == 2);

        if (user == null)
            throw new Exception("Организация не найдена");

        if (dto.Name != null)
            user.Name = dto.Name;

        if (dto.Description != null)
            user.Description = dto.Description;

        if (dto.Location != null)
        {
            var locationId = await GetLocationIdByNameAsync(dto.Location);
            user.LocationId = locationId;
        }

        if (user.OrganizationDetails != null)
        {
            if (dto.Phone != null)
                user.OrganizationDetails.Phone = dto.Phone;
            if (dto.Website != null)
                user.OrganizationDetails.Website = dto.Website;
            if (dto.DonationDetails != null)
                user.OrganizationDetails.DonationDetails = dto.DonationDetails;
        }

        if (dto.ConstantNeeds != null)
        {
            await UpdateOrganizationConstantNeedsAsync(userId, dto.ConstantNeeds);
        }

        if (dto.Photo != null)
        {
            var photoUrl = await _imageKitService.UploadImageAsync(dto.Photo, "organizations", userId.ToString());
            if (photoUrl != null)
                user.PhotoUrl = photoUrl;
        }
        else if (!string.IsNullOrEmpty(dto.PhotoUrlFromWeb))
        {
            var photoUrl = await _imageKitService.UploadImageFromUrlAsync(dto.PhotoUrlFromWeb, "organizations", userId.ToString());
            if (photoUrl != null)
                user.PhotoUrl = photoUrl;
        }

        await _context.SaveChangesAsync();

        var constantNeeds = await GetOrganizationConstantNeedsAsync(userId);
        var latestComments = await GetLatestCommentsAsync(userId, 5);
        var latestPost = await _postService.GetLatestPostByOrganizationAsync(userId);
        var locationName = await GetLocationNameByIdAsync(user.LocationId);
        var latestAnimals = await _animalService.GetLatestUserAnimalsAsync(userId);
        var roleName = await GetRoleNameByIdAsync(user.RoleId);

        return new OrganizationResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = roleName,
            Phone = user.OrganizationDetails?.Phone,
            Website = user.OrganizationDetails?.Website,
            DonationDetails = user.OrganizationDetails?.DonationDetails,
            Description = user.Description,
            Location = locationName,
            PhotoUrl = user.PhotoUrl,
            ConstantNeeds = constantNeeds,
            LatestComments = latestComments,
            LatestPost = latestPost,
            LatestAnimals = latestAnimals,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<OrganizationResponseDto> GetPublicOrganizationProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Location)
            .Include(u => u.OrganizationDetails)
            .FirstOrDefaultAsync(u => u.Id == userId && u.RoleId == 2);

        if (user == null)
            throw new Exception("Организация не найдена");

        var constantNeeds = await GetOrganizationConstantNeedsAsync(userId);
        var latestComments = await GetLatestCommentsAsync(userId, 5);
        var latestPost = await _postService.GetLatestPostByOrganizationAsync(userId);
        var locationName = await GetLocationNameByIdAsync(user.LocationId);
        var latestAnimals = await _animalService.GetLatestUserAnimalsAsync(userId);
        var roleName = await GetRoleNameByIdAsync(user.RoleId);

        return new OrganizationResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = roleName,
            Phone = user.OrganizationDetails?.Phone,
            Website = user.OrganizationDetails?.Website,
            DonationDetails = user.OrganizationDetails?.DonationDetails,
            Description = user.Description,
            Location = locationName,
            PhotoUrl = user.PhotoUrl,
            CountTasks = user.CountTasks,
            ConstantNeeds = constantNeeds,
            LatestComments = latestComments,
            LatestPost = latestPost,
            LatestAnimals = latestAnimals,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            CreatedAt = user.CreatedAt
        };
    }
}