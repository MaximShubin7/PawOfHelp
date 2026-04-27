// Services/Interfaces/IDictionaryService.cs
using PawOfHelp.DTOs.Dictionary;

namespace PawOfHelp.Services.Interfaces;

public interface IDictionaryService
{
    Task<List<DictionaryResponseDto>> GetLocationsAsync();
    Task<List<DictionaryResponseDto>> GetCompetenciesAsync();
    Task<List<DictionaryResponseDto>> GetAvailabilitiesAsync();
    Task<List<DictionaryResponseDto>> GetPreferencesAsync();
    Task<List<DictionaryResponseDto>> GetConstantNeedsAsync();
    Task<List<DictionaryResponseDto>> GetAnimalTypesAsync();
    Task<List<DictionaryResponseDto>> GetThemesAsync();
    Task<List<DictionaryResponseDto>> GetStatusesAsync();
}