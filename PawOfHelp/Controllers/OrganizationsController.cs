// Controllers/OrganizationsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PawOfHelp.DTOs.Organization;
using PawOfHelp.Services.Interfaces;
using System.Text.Json;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var profile = await _organizationService.GetOrganizationAsync(userId);
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
            UpdateOrganizationDto? dto = null;

            if (Request.HasFormContentType)
            {
                dto = new UpdateOrganizationDto
                {
                    Name = Request.Form["Name"],
                    Phone = Request.Form["Phone"],
                    Website = Request.Form["Website"],
                    DonationDetails = Request.Form["DonationDetails"],
                    Description = Request.Form["Description"],
                    Location = Request.Form["Location"],
                    Photo = Request.Form.Files.GetFile("Photo"),
                    PhotoUrlFromWeb = Request.Form["PhotoUrlFromWeb"]
                };

                var constantNeedsJson = Request.Form["ConstantNeeds"].ToString();
                if (!string.IsNullOrEmpty(constantNeedsJson))
                {
                    dto.ConstantNeeds = JsonSerializer.Deserialize<List<string>>(constantNeedsJson);
                }
            }
            else if (Request.ContentType?.Contains("application/json") == true)
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                dto = JsonSerializer.Deserialize<UpdateOrganizationDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (dto == null)
                return BadRequest("Неверный формат запроса");

            var result = await _organizationService.UpdateOrganizationAsync(userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}