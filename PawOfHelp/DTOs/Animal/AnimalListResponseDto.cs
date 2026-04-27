// DTOs/Animal/AnimalListResponseDto.cs
namespace PawOfHelp.DTOs.Animal;

public class AnimalListResponseDto
{
    public List<AnimalShortResponseDto> Animals { get; set; } = new();
    public int Offset { get; set; }
    public int Limit { get; set; }
    public bool HasMore { get; set; }
}