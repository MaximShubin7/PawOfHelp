// Models/Animal.cs
namespace PawOfHelp.Models;

public class Animal
{
    public Guid Id { get; set; }
    public short AnimalTypeId { get; set; }
    public string? Breed { get; set; }
    public string Name { get; set; } = string.Empty;
    public short? Age { get; set; }
    public string? Health { get; set; }
    public string? Character { get; set; }
    public string? SpecialNeeds { get; set; }
    public string? PhotoUrl { get; set; }

    public AnimalType AnimalType { get; set; } = null!;
}