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

    private async Task<List<short>> GetThemeIdsByNamesAsync(List<string> themeNames)
    {
        var themeIds = new List<short>();

        foreach (var themeName in themeNames.Distinct())
        {
            var theme = await _context.Themes
                .FirstOrDefaultAsync(t => t.Name == themeName);

            if (theme == null)
                throw new Exception($"Тема '{themeName}' не найдена");

            themeIds.Add(theme.Id);
        }

        return themeIds;
    }

    public async Task<List<ReferenceBookResponseDto>> GetByAnimalTypeAndThemesAsync(ReferenceBookRequestDto request)
    {
        var animalType = await _context.AnimalTypes
            .FirstOrDefaultAsync(at => at.Name == request.AnimalType);

        if (animalType == null)
            throw new Exception($"Тип животного '{request.AnimalType}' не найден");

        var query = _context.ReferenceBook
            .Where(rb => rb.AnimalTypeId == animalType.Id);

        if (request.Themes != null && request.Themes.Any())
        {
            var themeIds = await GetThemeIdsByNamesAsync(request.Themes);
            query = query.Where(rb => themeIds.Contains(rb.ThemeId));
        }

        var references = await query
            .Include(rb => rb.Theme)
            .ToListAsync();

        if (!references.Any())
            throw new Exception($"Справочная информация для типа животного '{request.AnimalType}' не найдена");

        return references.Select(rb => new ReferenceBookResponseDto
        {
            Title = rb.Title,
            Description = rb.Description,
            VideoUrl = rb.VideoUrl
        }).ToList();
    }
}