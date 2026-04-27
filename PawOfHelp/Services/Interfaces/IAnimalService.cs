// Services/Interfaces/IAnimalService.cs
using PawOfHelp.DTOs.Animal;

namespace PawOfHelp.Services.Interfaces;

public interface IAnimalService
{
    Task<AnimalResponseDto> CreateAnimalAsync(Guid userId, CreateAnimalDto dto);
    Task<AnimalResponseDto> UpdateAnimalAsync(Guid animalId, Guid userId, UpdateAnimalDto dto);
    Task DeleteAnimalAsync(Guid animalId, Guid userId);
    Task<AnimalResponseDto> GetAnimalByIdAsync(Guid animalId);
    Task<AnimalListResponseDto> GetOrganizationAnimalsAsync(Guid organizationId, int offset, int limit);
    Task<AnimalListResponseDto> GetVolunteerAnimalsAsync(Guid volunteerId, int offset, int limit);
    Task<List<AnimalShortResponseDto>> GetLatestUserAnimalsAsync(Guid userId);
}