// DTOs/ReferenceBook/ReferenceBookResponseDto.cs
namespace PawOfHelp.DTOs.ReferenceBook;

public class ReferenceBookResponseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
}