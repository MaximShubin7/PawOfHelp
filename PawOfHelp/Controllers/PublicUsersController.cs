// Controllers/PublicUsersController.cs
using Microsoft.AspNetCore.Mvc;
using PawOfHelp.DTOs.User;
using PawOfHelp.DTOs.Organization;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicUsersController : ControllerBase
{
    private readonly IPublicUserService _publicUserService;

    public PublicUsersController(IPublicUserService publicUserService)
    {
        _publicUserService = publicUserService;
    }

    [HttpGet("volunteer/{userId}")]
    public async Task<IActionResult> GetPublicVolunteerProfile(Guid userId)
    {
        try
        {
            var profile = await _publicUserService.GetPublicUserProfileAsync(userId);
            return Ok(profile);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("organization/{userId}")]
    public async Task<IActionResult> GetPublicOrganizationProfile(Guid userId)
    {
        try
        {
            var profile = await _publicUserService.GetPublicOrganizationProfileAsync(userId);
            return Ok(profile);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}