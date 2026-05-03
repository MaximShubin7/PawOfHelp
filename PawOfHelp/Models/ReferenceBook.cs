// Models/ReferenceBook.cs
namespace PawOfHelp.Models;

public class ReferenceBook
{
    public short AnimalTypeId { get; set; }
    public short ThemeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }

    public AnimalType AnimalType { get; set; } = null!;
    public Theme Theme { get; set; } = null!;
}