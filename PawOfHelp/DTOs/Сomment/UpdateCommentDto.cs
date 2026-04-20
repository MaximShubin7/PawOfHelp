// DTOs/Comment/UpdateCommentDto.cs
namespace PawOfHelp.DTOs.Comment;

public class UpdateCommentDto
{
    public short? Rating { get; set; }
    public string? Description { get; set; }
}