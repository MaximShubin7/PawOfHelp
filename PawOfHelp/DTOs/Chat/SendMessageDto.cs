namespace PawOfHelp.DTOs.Chat;

public class SendMessageDto
{
    public Guid TaskId { get; set; }
    public string Message { get; set; } = string.Empty;
}