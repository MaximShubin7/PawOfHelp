// Controllers/ReferenceBookController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using PawOfHelp.DTOs.ReferenceBook;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class ReferenceBookController : ControllerBase
{
    private readonly IReferenceBookService _referenceBookService;
    private readonly IValidator<ReferenceBookRequestDto> _validator;

    public ReferenceBookController(
        IReferenceBookService referenceBookService,
        IValidator<ReferenceBookRequestDto> validator)
    {
        _referenceBookService = referenceBookService;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ReferenceBookRequestDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.First().ErrorMessage);

        try
        {
            var result = await _referenceBookService.GetByAnimalTypeAndThemeAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}