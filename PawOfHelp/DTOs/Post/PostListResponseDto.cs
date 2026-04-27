// DTOs/Post/PostListResponseDto.cs
namespace PawOfHelp.DTOs.Post;

public class PostListResponseDto
{
    public List<PostResponseDto> Posts { get; set; } = new();
    public int Offset { get; set; }
    public int Limit { get; set; }
    public bool HasMore { get; set; }
}