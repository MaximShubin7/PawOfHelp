namespace PawOfHelp.DTOs.Chat;

public class ChatListResponseDto
{
    public List<ChatRoomResponseDto> Chats { get; set; } = new();
    public int Offset { get; set; }
    public int Limit { get; set; }
    public bool HasMore { get; set; }
}