// DTOs/Animal/CreateAnimalDto.cs
namespace PawOfHelp.DTOs.Animal;

public class CreateAnimalDto
{
    public string AnimalType { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string Name { get; set; } = string.Empty;
    public short? Age { get; set; }
    public string? Health { get; set; }
    public string? Character { get; set; }
    public string? SpecialNeeds { get; set; }
    public IFormFile? Photo { get; set; }
    public string? PhotoUrlFromWeb { get; set; }
}