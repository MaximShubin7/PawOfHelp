// DTOs/Organization/OrganizationResponseDto.cs
namespace PawOfHelp.DTOs.Organization;

public class OrganizationResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? DonationDetails { get; set; }
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
    public int SumRating { get; set; }
    public int CountRating { get; set; }
    public double AverageRating => CountRating > 0 ? (double)SumRating / CountRating : 0;
    public short Role { get; set; }
    public DateTime CreatedAt { get; set; }
}