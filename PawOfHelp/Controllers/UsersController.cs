// Controllers/UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using PawOfHelp.DTOs.User;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IOrganizationService _organizationService;

    public UsersController(IUserService userService, IOrganizationService organizationService)
    {
        _userService = userService;
        _organizationService = organizationService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var profile = await _userService.GetProfileAsync(userId);
            return Ok(profile);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            UpdateProfileDto? dto = null;

            if (Request.HasFormContentType)
            {
                dto = new UpdateProfileDto
                {
                    Name = Request.Form["Name"],
                    Age = short.TryParse(Request.Form["Age"], out var age) ? age : null,
                    Description = Request.Form["Description"],
                    Location = Request.Form["Location"],
                    Photo = Request.Form.Files.GetFile("Photo"),
                    PhotoUrlFromWeb = Request.Form["PhotoUrlFromWeb"]
                };

                var competencyNamesJson = Request.Form["CompetencyNames"].ToString();
                if (!string.IsNullOrEmpty(competencyNamesJson))
                {
                    dto.Competencies = JsonSerializer.Deserialize<List<string>>(competencyNamesJson);
                }

                var preferenceNamesJson = Request.Form["PreferenceNames"].ToString();
                if (!string.IsNullOrEmpty(preferenceNamesJson))
                {
                    dto.Preferences = JsonSerializer.Deserialize<List<string>>(preferenceNamesJson);
                }

                var availabilityNamesJson = Request.Form["AvailabilityNames"].ToString();
                if (!string.IsNullOrEmpty(availabilityNamesJson))
                {
                    dto.Availabilities = JsonSerializer.Deserialize<List<string>>(availabilityNamesJson);
                }
            }
            else if (Request.ContentType?.Contains("application/json") == true)
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                dto = JsonSerializer.Deserialize<UpdateProfileDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (dto == null)
                return BadRequest("Неверный формат запроса");

            var result = await _userService.UpdateProfileAsync(userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("profile")]
    public async Task<IActionResult> DeleteProfile()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            await _userService.DeleteProfileAsync(userId);
            return Ok(new { message = "Аккаунт успешно удалён" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            await _userService.ChangePasswordAsync(userId, dto);
            return Ok(new { message = "Пароль успешно изменён" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("public/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicProfile(Guid userId)
    {
        try
        {
            var role = await _userService.GetUserRoleAsync(userId);

            if (role == "Организация")
            {
                var result = await _organizationService.GetPublicOrganizationProfileAsync(userId);
                return Ok(result);
            }

            var userResult = await _userService.GetPublicUserProfileAsync(userId);
            return Ok(userResult);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}