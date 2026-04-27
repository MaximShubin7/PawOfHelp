// DTOs/Animal/AnimalResponseDto.cs
namespace PawOfHelp.DTOs.Animal;

public class AnimalResponseDto
{
    public Guid Id { get; set; }
    public string AnimalType { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string Name { get; set; } = string.Empty;
    public short? Age { get; set; }
    public string? Health { get; set; }
    public string? Character { get; set; }
    public string? SpecialNeeds { get; set; }
    public string? PhotoUrl { get; set; }
    public List<AnimalOwnerShortDto> Owners { get; set; } = new();
}