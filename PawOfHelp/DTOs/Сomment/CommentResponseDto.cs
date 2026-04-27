// DTOs/Comment/CommentResponseDto.cs
using PawOfHelp.DTOs.Public;

namespace PawOfHelp.DTOs.Comment;

public class CommentResponseDto
{
    public Guid Id { get; set; }
    public short Rating { get; set; }
    public string? Description { get; set; }
    public PublicProfileDto Sender { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}