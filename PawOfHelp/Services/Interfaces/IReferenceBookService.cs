// Services/Interfaces/IReferenceBookService.cs
using PawOfHelp.DTOs.ReferenceBook;

namespace PawOfHelp.Services.Interfaces;

public interface IReferenceBookService
{
    Task<ReferenceBookResponseDto> GetByAnimalTypeAndThemeAsync(ReferenceBookRequestDto request);
}