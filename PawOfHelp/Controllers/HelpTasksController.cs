// Controllers/HelpTasksController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FluentValidation;
using PawOfHelp.DTOs.HelpTask;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HelpTasksController : ControllerBase
{
    private readonly IHelpTaskService _helpTaskService;
    private readonly IValidator<CreateHelpTaskDto> _createValidator;
    private readonly IValidator<UpdateHelpTaskDto> _updateValidator;

    public HelpTasksController(
        IHelpTaskService helpTaskService,
        IValidator<CreateHelpTaskDto> createValidator,
        IValidator<UpdateHelpTaskDto> updateValidator)
    {
        _helpTaskService = helpTaskService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHelpTaskDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.First().ErrorMessage);

        try
        {
            var result = await _helpTaskService.CreateHelpTaskAsync(userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{taskId}")]
    public async Task<IActionResult> Update(Guid taskId, [FromBody] UpdateHelpTaskDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.First().ErrorMessage);

        try
        {
            var result = await _helpTaskService.UpdateHelpTaskAsync(taskId, userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> Delete(Guid taskId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            await _helpTaskService.DeleteHelpTaskAsync(taskId, userId);
            return Ok(new { message = "Задача успешно удалена" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{taskId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid taskId)
    {
        try
        {
            var result = await _helpTaskService.GetHelpTaskByIdAsync(taskId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed([FromQuery] HelpTaskFilterDto filter, [FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var result = await _helpTaskService.GetFeedTasksAsync(userId, filter, offset, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my-created")]
    public async Task<IActionResult> GetMyCreated([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var result = await _helpTaskService.GetTasksByCreatorAsync(userId, offset, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my-working")]
    public async Task<IActionResult> GetMyWorking([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var result = await _helpTaskService.GetTasksByWorkerAsync(userId, offset, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{taskId}/complete")]
    public async Task<IActionResult> Complete(Guid taskId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            await _helpTaskService.CompleteAndDeleteTaskAsync(taskId, userId);
            return Ok(new { message = "Задача выполнена и удалена" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}