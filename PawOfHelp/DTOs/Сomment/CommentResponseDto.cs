// DTOs/Comment/CommentResponseDto.cs
namespace PawOfHelp.DTOs.Comment;

public class CommentResponseDto
{
    public Guid Id { get; set; }
    public short Rating { get; set; }
    public string? Description { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public Guid RecipientId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}