// Models/Comment.cs
namespace PawOfHelp.Models;

public class Comment
{
    public Guid Id { get; set; }
    public short Rating { get; set; }
    public string? Description { get; set; }
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public DateTime CreatedAt { get; set; }
    public User Sender { get; set; } = null!;
    public User Recipient { get; set; } = null!;
}