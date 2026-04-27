// Controllers/AnimalsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using FluentValidation;
using PawOfHelp.DTOs.Animal;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnimalsController : ControllerBase
{
    private readonly IAnimalService _animalService;
    private readonly IUserService _userService;
    private readonly IValidator<CreateAnimalDto> _createValidator;
    private readonly IValidator<UpdateAnimalDto> _updateValidator;

    public AnimalsController(
        IAnimalService animalService,
        IUserService userService,
        IValidator<CreateAnimalDto> createValidator,
        IValidator<UpdateAnimalDto> updateValidator)
    {
        _animalService = animalService;
        _userService = userService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAnimal()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            CreateAnimalDto? dto = null;

            if (Request.HasFormContentType)
            {
                dto = new CreateAnimalDto
                {
                    AnimalType = Request.Form["AnimalType"],
                    Breed = Request.Form["Breed"],
                    Name = Request.Form["Name"],
                    Age = string.IsNullOrEmpty(Request.Form["Age"]) ? null : short.Parse(Request.Form["Age"]),
                    Health = Request.Form["Health"],
                    Character = Request.Form["Character"],
                    SpecialNeeds = Request.Form["SpecialNeeds"],
                    Photo = Request.Form.Files.GetFile("Photo"),
                    PhotoUrlFromWeb = Request.Form["PhotoUrlFromWeb"]
                };
            }
            else if (Request.ContentType?.Contains("application/json") == true)
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                dto = JsonSerializer.Deserialize<CreateAnimalDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (dto == null)
                return BadRequest("Неверный формат запроса");

            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.First().ErrorMessage);

            var result = await _animalService.CreateAnimalAsync(userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{animalId}")]
    public async Task<IActionResult> UpdateAnimal(Guid animalId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            UpdateAnimalDto? dto = null;

            if (Request.HasFormContentType)
            {
                dto = new UpdateAnimalDto
                {
                    AnimalType = Request.Form["AnimalType"],
                    Breed = Request.Form["Breed"],
                    Name = Request.Form["Name"],
                    Age = string.IsNullOrEmpty(Request.Form["Age"]) ? null : short.Parse(Request.Form["Age"]),
                    Health = Request.Form["Health"],
                    Character = Request.Form["Character"],
                    SpecialNeeds = Request.Form["SpecialNeeds"],
                    Photo = Request.Form.Files.GetFile("Photo"),
                    PhotoUrlFromWeb = Request.Form["PhotoUrlFromWeb"]
                };
            }
            else if (Request.ContentType?.Contains("application/json") == true)
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                dto = JsonSerializer.Deserialize<UpdateAnimalDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (dto == null)
                return BadRequest("Неверный формат запроса");

            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.First().ErrorMessage);

            var result = await _animalService.UpdateAnimalAsync(animalId, userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{animalId}")]
    public async Task<IActionResult> DeleteAnimal(Guid animalId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            await _animalService.DeleteAnimalAsync(animalId, userId);
            return Ok(new { message = "Животное успешно удалено" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{animalId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAnimalById(Guid animalId)
    {
        try
        {
            var result = await _animalService.GetAnimalByIdAsync(animalId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("organization/{organizationId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrganizationAnimals(Guid organizationId, [FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _animalService.GetOrganizationAnimalsAsync(organizationId, offset, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("volunteer/{volunteerId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVolunteerAnimals(Guid volunteerId, [FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _animalService.GetVolunteerAnimalsAsync(volunteerId, offset, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAnimals([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var role = await _userService.GetUserRoleAsync(userId);

            if (role == "Организация")
            {
                var result = await _animalService.GetOrganizationAnimalsAsync(userId, offset, limit);
                return Ok(result);
            }
            else
            {
                var result = await _animalService.GetVolunteerAnimalsAsync(userId, offset, limit);
                return Ok(result);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}