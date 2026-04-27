// DTOs/Auth/AuthResponseDto.cs
using PawOfHelp.DTOs.Animal;
using PawOfHelp.DTOs.Comment;

namespace PawOfHelp.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public short? Age { get; set; }
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Location { get; set; }
    public int CountTasks { get; set; }
    public List<CommentResponseDto> LatestComments { get; set; } = new();
    public List<AnimalShortResponseDto> LatestAnimals { get; set; } = new();
    public int SumRating { get; set; }
    public int CountRating { get; set; }
    public DateTime CreatedAt { get; set; }
}