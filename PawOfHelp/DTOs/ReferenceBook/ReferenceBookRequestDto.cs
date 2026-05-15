// DTOs/ReferenceBook/ReferenceBookRequestDto.cs
namespace PawOfHelp.DTOs.ReferenceBook;

public class ReferenceBookRequestDto
{
    public string AnimalType { get; set; } = string.Empty;
    public List<string>? Themes { get; set; }
}