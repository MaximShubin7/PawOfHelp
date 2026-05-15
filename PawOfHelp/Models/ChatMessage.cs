namespace PawOfHelp.Models;

public class ChatMessage
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid SenderId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ChatRoom ChatRoom { get; set; } = null!;
    public User Sender { get; set; } = null!;
}