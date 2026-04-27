// Controllers/ResponsesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FluentValidation;
using PawOfHelp.DTOs.Response;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResponsesController : ControllerBase
{
    private readonly IResponseService _responseService;
    private readonly IValidator<CreateResponseDto> _createValidator;
    private readonly IValidator<UpdateResponseStatusDto> _updateStatusValidator;

    public ResponsesController(
        IResponseService responseService,
        IValidator<CreateResponseDto> createValidator,
        IValidator<UpdateResponseStatusDto> updateStatusValidator)
    {
        _responseService = responseService;
        _createValidator = createValidator;
        _updateStatusValidator = updateStatusValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateResponseDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.First().ErrorMessage);

        try
        {
            var result = await _responseService.CreateResponseAsync(userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{responseId}/status")]
    public async Task<IActionResult> UpdateStatus(Guid responseId, [FromBody] UpdateResponseStatusDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var validationResult = await _updateStatusValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.First().ErrorMessage);

        try
        {
            var result = await _responseService.UpdateResponseStatusAsync(responseId, userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetByTask(Guid taskId, [FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var result = await _responseService.GetResponsesByTaskAsync(taskId, userId, offset, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my-received")]
    public async Task<IActionResult> GetMyReceived([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var result = await _responseService.GetResponsesByCreatorAsync(userId, offset, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my-sent")]
    public async Task<IActionResult> GetMySent([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var result = await _responseService.GetResponsesBySenderAsync(userId, offset, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}