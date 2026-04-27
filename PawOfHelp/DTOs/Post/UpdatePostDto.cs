// DTOs/Post/UpdatePostDto.cs
namespace PawOfHelp.DTOs.Post;

public class UpdatePostDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<IFormFile>? NewPhotos { get; set; }
}