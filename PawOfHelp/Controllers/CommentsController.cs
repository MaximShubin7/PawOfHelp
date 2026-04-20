// Controllers/CommentsController.cs (обновлённый с валидацией)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FluentValidation;
using PawOfHelp.DTOs.Comment;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IValidator<CreateCommentDto> _createCommentValidator;
    private readonly IValidator<UpdateCommentDto> _updateCommentValidator;

    public CommentsController(
        ICommentService commentService,
        IValidator<CreateCommentDto> createCommentValidator,
        IValidator<UpdateCommentDto> updateCommentValidator)
    {
        _commentService = commentService;
        _createCommentValidator = createCommentValidator;
        _updateCommentValidator = updateCommentValidator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
    {
        var validationResult = await _createCommentValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.First().ErrorMessage);

        var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var result = await _commentService.CreateCommentAsync(senderId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{commentId}")]
    public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentDto dto)
    {
        var validationResult = await _updateCommentValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.First().ErrorMessage);

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var result = await _commentService.UpdateCommentAsync(commentId, userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            await _commentService.DeleteCommentAsync(commentId, userId);
            return Ok(new { message = "Комментарий успешно удалён" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserComments(Guid userId, [FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _commentService.GetUserCommentsAsync(userId, offset, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}