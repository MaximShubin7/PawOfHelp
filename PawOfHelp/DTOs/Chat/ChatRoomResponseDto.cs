namespace PawOfHelp.DTOs.Chat;

public class ChatRoomResponseDto
{
    public Guid TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public bool HasNewMessages { get; set; }
}