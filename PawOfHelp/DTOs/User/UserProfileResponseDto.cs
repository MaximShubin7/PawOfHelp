// DTOs/User/UserProfileResponseDto.cs
using PawOfHelp.DTOs.Animal;
using PawOfHelp.DTOs.Comment;

namespace PawOfHelp.DTOs.User;

public class UserProfileResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public short? Age { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? PhotoUrl { get; set; }
    public int CountTasks { get; set; }
    public List<string> Competencies { get; set; } = new();
    public List<string> Preferences { get; set; } = new();
    public List<string> Availabilities { get; set; } = new();
    public List<CommentResponseDto> LatestComments { get; set; } = new();
    public List<AnimalShortResponseDto> LatestAnimals { get; set; } = new();
    public int SumRating { get; set; }
    public int CountRating { get; set; }
    public double AverageRating => CountRating > 0 ? (double)SumRating / CountRating : 0;
    public DateTime CreatedAt { get; set; }
}