// DTOs/Post/CreatePostDto.cs
namespace PawOfHelp.DTOs.Post;

public class CreatePostDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<IFormFile>? Photos { get; set; }
}