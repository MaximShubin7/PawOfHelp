// Services/DictionaryService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Dictionary;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class DictionaryService : IDictionaryService
{
    private readonly AppDbContext _context;

    public DictionaryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DictionaryResponseDto>> GetLocationsAsync()
    {
        return await _context.Locations
            .OrderBy(l => l.Name)
            .Select(l => new DictionaryResponseDto
            {
                Id = l.Id,
                Name = l.Name
            })
            .ToListAsync();
    }

    public async Task<List<DictionaryResponseDto>> GetCompetenciesAsync()
    {
        return await _context.Competencies
            .OrderBy(c => c.Name)
            .Select(c => new DictionaryResponseDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<List<DictionaryResponseDto>> GetAvailabilitiesAsync()
    {
        return await _context.Availabilities
            .OrderBy(a => a.Name)
            .Select(a => new DictionaryResponseDto
            {
                Id = a.Id,
                Name = a.Name
            })
            .ToListAsync();
    }

    public async Task<List<DictionaryResponseDto>> GetPreferencesAsync()
    {
        return await _context.Preferences
            .OrderBy(p => p.Name)
            .Select(p => new DictionaryResponseDto
            {
                Id = p.Id,
                Name = p.Name
            })
            .ToListAsync();
    }

    public async Task<List<DictionaryResponseDto>> GetConstantNeedsAsync()
    {
        return await _context.ConstantNeeds
            .OrderBy(cn => cn.Name)
            .Select(cn => new DictionaryResponseDto
            {
                Id = cn.Id,
                Name = cn.Name
            })
            .ToListAsync();
    }

    public async Task<List<DictionaryResponseDto>> GetAnimalTypesAsync()
    {
        return await _context.AnimalTypes
            .OrderBy(at => at.Name)
            .Select(at => new DictionaryResponseDto
            {
                Id = at.Id,
                Name = at.Name
            })
            .ToListAsync();
    }

    public async Task<List<DictionaryResponseDto>> GetThemesAsync()
    {
        return await _context.Themes
            .OrderBy(t => t.Name)
            .Select(t => new DictionaryResponseDto
            {
                Id = t.Id,
                Name = t.Name
            })
            .ToListAsync();
    }

    public async Task<List<DictionaryResponseDto>> GetStatusesAsync()
    {
        return await _context.Statuses
            .OrderBy(s => s.Id)
            .Select(s => new DictionaryResponseDto
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();
    }
}