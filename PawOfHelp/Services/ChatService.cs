using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Chat;
using PawOfHelp.DTOs.Public;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class ChatService : IChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }

    private async Task EnsureChatRoomExistsAsync(Guid taskId)
    {
        var exists = await _context.ChatRooms.AnyAsync(r => r.TaskId == taskId);
        if (!exists)
        {
            _context.ChatRooms.Add(new ChatRoom { TaskId = taskId, LastMessageAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();
        }
    }

    private async Task EnsureParticipantExistsAsync(Guid taskId, Guid userId)
    {
        var exists = await _context.ChatParticipants
            .AnyAsync(p => p.TaskId == taskId && p.UserId == userId);

        if (!exists)
        {
            _context.ChatParticipants.Add(new ChatParticipant
            {
                TaskId = taskId,
                UserId = userId,
                LastReadAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ChatMessageResponseDto> SendMessageAsync(Guid senderId, SendMessageDto dto)
    {
        var task = await _context.HelpTasks.FirstOrDefaultAsync(t => t.Id == dto.TaskId);
        if (task == null)
            throw new Exception("Задача не найдена");

        await EnsureChatRoomExistsAsync(dto.TaskId);
        await EnsureParticipantExistsAsync(dto.TaskId, senderId);

        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            TaskId = dto.TaskId,
            SenderId = senderId,
            Message = dto.Message,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);

        var chatRoom = await _context.ChatRooms.FindAsync(dto.TaskId);
        if (chatRoom != null)
        {
            chatRoom.LastMessageAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(senderId);

        return new ChatMessageResponseDto
        {
            Id = message.Id,
            Sender = new PublicProfileDto
            {
                Id = sender!.Id,
                Name = sender.Name
            },
            Message = message.Message,
            CreatedAt = message.CreatedAt,
            IsNew = true
        };
    }

    public async Task<ChatListResponseDto> GetUserChatsAsync(Guid userId, int offset, int limit)
    {
        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var query = _context.ChatParticipants
            .Where(p => p.UserId == userId)
            .Join(_context.ChatRooms,
                p => p.TaskId,
                r => r.TaskId,
                (p, r) => new
                {
                    TaskId = r.TaskId,
                    TaskTitle = _context.HelpTasks.Where(t => t.Id == r.TaskId).Select(t => t.Title).FirstOrDefault(),
                    LastMessageAt = r.LastMessageAt,
                    LastReadAt = p.LastReadAt
                });

        var totalCount = await query.CountAsync();

        var chats = await query
            .OrderByDescending(c => c.LastMessageAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var result = new List<ChatRoomResponseDto>();
        foreach (var chat in chats)
        {
            bool hasNewMessages = await _context.ChatMessages
                .AnyAsync(m => m.TaskId == chat.TaskId && m.SenderId != userId && m.CreatedAt > chat.LastReadAt);

            result.Add(new ChatRoomResponseDto
            {
                TaskId = chat.TaskId,
                TaskTitle = chat.TaskTitle ?? string.Empty,
                HasNewMessages = hasNewMessages
            });
        }

        return new ChatListResponseDto
        {
            Chats = result,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }

    public async Task<ChatMessageListResponseDto> GetMessagesAsync(Guid taskId, Guid userId, int offset, int limit)
    {
        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var isParticipant = await _context.ChatParticipants
            .AnyAsync(p => p.TaskId == taskId && p.UserId == userId);

        if (!isParticipant)
            throw new Exception("Вы не участвуете в этом чате");

        var lastReadAt = await _context.ChatParticipants
            .Where(p => p.TaskId == taskId && p.UserId == userId)
            .Select(p => p.LastReadAt)
            .FirstOrDefaultAsync();

        var totalCount = await _context.ChatMessages
            .Where(m => m.TaskId == taskId)
            .CountAsync();

        var messages = await _context.ChatMessages
            .Where(m => m.TaskId == taskId)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var result = new List<ChatMessageResponseDto>();
        foreach (var message in messages.OrderBy(m => m.CreatedAt))
        {
            result.Add(new ChatMessageResponseDto
            {
                Id = message.Id,
                Sender = new PublicProfileDto
                {
                    Id = message.Sender.Id,
                    Name = message.Sender.Name
                },
                Message = message.Message,
                CreatedAt = message.CreatedAt,
                IsNew = message.SenderId != userId && message.CreatedAt > lastReadAt
            });
        }

        return new ChatMessageListResponseDto
        {
            Messages = result,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }

    public async Task MarkAsReadAsync(Guid taskId, Guid userId)
    {
        var participant = await _context.ChatParticipants
            .FirstOrDefaultAsync(p => p.TaskId == taskId && p.UserId == userId);

        if (participant != null)
        {
            participant.LastReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}