namespace PawOfHelp.Models;

public class ChatRoom
{
    public Guid TaskId { get; set; }
    public DateTime LastMessageAt { get; set; }
    public HelpTask Task { get; set; } = null!;
    public ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}