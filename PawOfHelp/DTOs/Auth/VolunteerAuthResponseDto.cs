// DTOs/Auth/VolunteerAuthResponseDto.cs
namespace PawOfHelp.DTOs.Auth;

public class VolunteerAuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public short Role { get; set; }
    public short? Age { get; set; }
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
    public int SumRating { get; set; }
    public int CountRating { get; set; }
    public double AverageRating => CountRating > 0 ? (double)SumRating / CountRating : 0;
    public DateTime CreatedAt { get; set; }
}