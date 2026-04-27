// Controllers/PostsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using FluentValidation;
using PawOfHelp.DTOs.Post;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IValidator<CreatePostDto> _createPostValidator;
    private readonly IValidator<UpdatePostDto> _updatePostValidator;

    public PostsController(
        IPostService postService,
        IValidator<CreatePostDto> createPostValidator,
        IValidator<UpdatePostDto> updatePostValidator)
    {
        _postService = postService;
        _createPostValidator = createPostValidator;
        _updatePostValidator = updatePostValidator;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            CreatePostDto? dto = null;

            if (Request.HasFormContentType)
            {
                dto = new CreatePostDto
                {
                    Title = Request.Form["Title"],
                    Description = Request.Form["Description"],
                    Photos = Request.Form.Files.GetFiles("Photos").ToList()
                };
            }
            else if (Request.ContentType?.Contains("application/json") == true)
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                dto = JsonSerializer.Deserialize<CreatePostDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (dto == null)
                return BadRequest("Неверный формат запроса");

            var validationResult = await _createPostValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.First().ErrorMessage);

            var result = await _postService.CreatePostAsync(userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{postId}")]
    [Authorize]
    public async Task<IActionResult> UpdatePost(Guid postId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            UpdatePostDto? dto = null;

            if (Request.HasFormContentType)
            {
                dto = new UpdatePostDto
                {
                    Title = Request.Form["Title"],
                    Description = Request.Form["Description"],
                    NewPhotos = Request.Form.Files.GetFiles("NewPhotos").ToList()
                };
            }
            else if (Request.ContentType?.Contains("application/json") == true)
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                dto = JsonSerializer.Deserialize<UpdatePostDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (dto == null)
                return BadRequest("Неверный формат запроса");

            var validationResult = await _updatePostValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.First().ErrorMessage);

            var result = await _postService.UpdatePostAsync(postId, userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{postId}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            await _postService.DeletePostAsync(postId, userId);
            return Ok(new { message = "Пост успешно удалён" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("organization/{organizationId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrganizationPosts(Guid organizationId, [FromQuery] int offset = 0, [FromQuery] int limit = 5)
    {
        try
        {
            var result = await _postService.GetOrganizationPostsAsync(organizationId, offset, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}