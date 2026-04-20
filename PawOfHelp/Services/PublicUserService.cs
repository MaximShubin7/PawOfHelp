// Services/PublicUserService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.User;
using PawOfHelp.DTOs.Organization;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class PublicUserService : IPublicUserService
{
    private readonly AppDbContext _context;

    public PublicUserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileResponseDto> GetPublicUserProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Пользователь не найден");

        if (user.Role == 2)
            throw new Exception("Это организация, используйте другой метод");

        return new UserProfileResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Age = user.Age,
            Description = user.Description,
            PhotoUrl = user.PhotoUrl,
            SumRating = user.SumRating,
            CountRating = user.CountRating,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<OrganizationResponseDto> GetPublicOrganizationProfileAsync(Guid userId)
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
}