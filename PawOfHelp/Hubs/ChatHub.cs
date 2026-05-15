using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Chat;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChatHub(IChatService chatService, IHttpContextAccessor httpContextAccessor)
    {
        _chatService = chatService;
        _httpContextAccessor = httpContextAccessor;
    }

    private Guid GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new Exception("User ID not found"));
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChat(Guid taskId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{taskId}");
    }

    public async Task LeaveChat(Guid taskId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{taskId}");
    }

    public async Task SendMessage(SendMessageDto dto)
    {
        var userId = GetUserId();
        var message = await _chatService.SendMessageAsync(userId, dto);

        await Clients.Group($"chat_{dto.TaskId}").SendAsync("ReceiveMessage", message);

        var otherParticipants = await GetOtherParticipants(dto.TaskId, userId);
        foreach (var participantId in otherParticipants)
        {
            await Clients.Group($"user_{participantId}").SendAsync("NewMessageNotification", new
            {
                TaskId = dto.TaskId,
                SenderName = message.Sender.Name,
                Message = dto.Message
            });
        }
    }

    private async Task<List<Guid>> GetOtherParticipants(Guid taskId, Guid currentUserId)
    {
        using var scope = _httpContextAccessor.HttpContext?.RequestServices.CreateScope();
        if (scope == null) return new List<Guid>();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var participants = await context.ChatParticipants
            .Where(p => p.TaskId == taskId && p.UserId != currentUserId)
            .Select(p => p.UserId)
            .ToListAsync<Guid>();

        return participants;
    }
}