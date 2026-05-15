namespace PawOfHelp.Models;

public class ChatParticipant
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public DateTime LastReadAt { get; set; }
    public ChatRoom ChatRoom { get; set; } = null!;
    public User User { get; set; } = null!;
}