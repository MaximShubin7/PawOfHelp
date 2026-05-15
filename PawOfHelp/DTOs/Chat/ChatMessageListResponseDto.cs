namespace PawOfHelp.DTOs.Chat;

public class ChatMessageListResponseDto
{
    public List<ChatMessageResponseDto> Messages { get; set; } = new();
    public int Offset { get; set; }
    public int Limit { get; set; }
    public bool HasMore { get; set; }
}