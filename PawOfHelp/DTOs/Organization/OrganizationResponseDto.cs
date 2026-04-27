// DTOs/Organization/OrganizationResponseDto.cs
using PawOfHelp.DTOs.Animal;
using PawOfHelp.DTOs.Comment;
using PawOfHelp.DTOs.Post;

namespace PawOfHelp.DTOs.Organization;

public class OrganizationResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? DonationDetails { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? PhotoUrl { get; set; }
    public int CountTasks { get; set; }
    public List<string> ConstantNeeds { get; set; } = new();
    public List<CommentResponseDto> LatestComments { get; set; } = new();
    public PostResponseDto? LatestPost { get; set; }
    public List<AnimalShortResponseDto> LatestAnimals { get; set; } = new();
    public int SumRating { get; set; }
    public int CountRating { get; set; }
    public double AverageRating => CountRating > 0 ? (double)SumRating / CountRating : 0;
    public DateTime CreatedAt { get; set; }
}