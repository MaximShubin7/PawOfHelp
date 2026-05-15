using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PawOfHelp.DTOs.Chat;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    }

    [HttpGet("chats")]
    public async Task<IActionResult> GetMyChats([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var userId = GetUserId();
        var result = await _chatService.GetUserChatsAsync(userId, offset, limit);
        return Ok(result);
    }

    [HttpGet("messages/{taskId}")]
    public async Task<IActionResult> GetMessages(Guid taskId, [FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var userId = GetUserId();
        var result = await _chatService.GetMessagesAsync(taskId, userId, offset, limit);
        return Ok(result);
    }

    [HttpPost("messages/read/{taskId}")]
    public async Task<IActionResult> MarkAsRead(Guid taskId)
    {
        var userId = GetUserId();
        await _chatService.MarkAsReadAsync(taskId, userId);
        return Ok(new { message = "Прочитано" });
    }
}