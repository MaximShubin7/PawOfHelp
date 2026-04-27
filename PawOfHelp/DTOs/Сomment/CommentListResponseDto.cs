// DTOs/Comment/CommentListResponseDto.cs
namespace PawOfHelp.DTOs.Comment;

public class CommentListResponseDto
{
    public List<CommentResponseDto> Comments { get; set; } = new();
    public int Offset { get; set; }
    public int Limit { get; set; }
    public bool HasMore { get; set; }
}