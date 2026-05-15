using PawOfHelp.DTOs.Chat;

namespace PawOfHelp.Services.Interfaces;

public interface IChatService
{
    Task<ChatMessageResponseDto> SendMessageAsync(Guid senderId, SendMessageDto dto);
    Task<ChatListResponseDto> GetUserChatsAsync(Guid userId, int offset, int limit);
    Task<ChatMessageListResponseDto> GetMessagesAsync(Guid taskId, Guid userId, int offset, int limit);
    Task MarkAsReadAsync(Guid taskId, Guid userId);
}