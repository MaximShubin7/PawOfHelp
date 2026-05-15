using PawOfHelp.DTOs.Public;

namespace PawOfHelp.DTOs.Chat;

public class ChatMessageResponseDto
{
    public Guid Id { get; set; }
    public PublicProfileDto Sender { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsNew { get; set; }
}