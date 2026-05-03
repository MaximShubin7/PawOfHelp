// Services/ReferenceBookService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.ReferenceBook;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class ReferenceBookService : IReferenceBookService
{
    private readonly AppDbContext _context;

    public ReferenceBookService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ReferenceBookResponseDto> GetByAnimalTypeAndThemeAsync(ReferenceBookRequestDto request)
    {
        var animalType = await _context.AnimalTypes
            .FirstOrDefaultAsync(at => at.Name == request.AnimalType);

        if (animalType == null)
            throw new Exception($"Тип животного '{request.AnimalType}' не найден");

        var theme = await _context.Themes
            .FirstOrDefaultAsync(t => t.Name == request.Theme);

        if (theme == null)
            throw new Exception($"Тема '{request.Theme}' не найдена");

        var reference = await _context.ReferenceBook
            .FirstOrDefaultAsync(rb => rb.AnimalTypeId == animalType.Id && rb.ThemeId == theme.Id);

        if (reference == null)
            throw new Exception($"Справочная информация для типа животного '{request.AnimalType}' и темы '{request.Theme}' не найдена");

        return new ReferenceBookResponseDto
        {
            Title = reference.Title,
            Description = reference.Description,
            VideoUrl = reference.VideoUrl
        };
    }
}