// Controllers/DictionariesController.cs
using Microsoft.AspNetCore.Mvc;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DictionariesController : ControllerBase
{
    private readonly IDictionaryService _dictionaryService;

    public DictionariesController(IDictionaryService dictionaryService)
    {
        _dictionaryService = dictionaryService;
    }

    [HttpGet("locations")]
    public async Task<IActionResult> GetLocations()
    {
        var locations = await _dictionaryService.GetLocationsAsync();
        return Ok(locations);
    }

    [HttpGet("competencies")]
    public async Task<IActionResult> GetCompetencies()
    {
        var competencies = await _dictionaryService.GetCompetenciesAsync();
        return Ok(competencies);
    }

    [HttpGet("availabilities")]
    public async Task<IActionResult> GetAvailabilities()
    {
        var availabilities = await _dictionaryService.GetAvailabilitiesAsync();
        return Ok(availabilities);
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        var preferences = await _dictionaryService.GetPreferencesAsync();
        return Ok(preferences);
    }

    [HttpGet("constant-needs")]
    public async Task<IActionResult> GetConstantNeeds()
    {
        var constantNeeds = await _dictionaryService.GetConstantNeedsAsync();
        return Ok(constantNeeds);
    }

    [HttpGet("animal-types")]
    public async Task<IActionResult> GetAnimalTypes()
    {
        var animalTypes = await _dictionaryService.GetAnimalTypesAsync();
        return Ok(animalTypes);
    }

    [HttpGet("themes")]
    public async Task<IActionResult> GetThemes()
    {
        var themes = await _dictionaryService.GetThemesAsync();
        return Ok(themes);
    }

    [HttpGet("statuses")]
    public async Task<IActionResult> GetStatuses()
    {
        var statuses = await _dictionaryService.GetStatusesAsync();
        return Ok(statuses);
    }
}