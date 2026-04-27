// DTOs/Post/PostResponseDto.cs
namespace PawOfHelp.DTOs.Post;

public class PostResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> PhotoUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}