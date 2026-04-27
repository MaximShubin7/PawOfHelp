// DTOs/Animal/UpdateAnimalDto.cs
namespace PawOfHelp.DTOs.Animal;

public class UpdateAnimalDto
{
    public string? AnimalType { get; set; }
    public string? Breed { get; set; }
    public string? Name { get; set; }
    public short? Age { get; set; }
    public string? Health { get; set; }
    public string? Character { get; set; }
    public string? SpecialNeeds { get; set; }
    public IFormFile? Photo { get; set; }
    public string? PhotoUrlFromWeb { get; set; }
}