// DTOs/Comment/CreateCommentDto.cs
namespace PawOfHelp.DTOs.Comment;

public class CreateCommentDto
{
    public short Rating { get; set; }
    public string? Description { get; set; }
    public Guid RecipientId { get; set; }
}