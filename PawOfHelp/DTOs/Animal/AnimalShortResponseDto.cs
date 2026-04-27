// DTOs/Animal/AnimalShortResponseDto.cs
namespace PawOfHelp.DTOs.Animal;

public class AnimalShortResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}