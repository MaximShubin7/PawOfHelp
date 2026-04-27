// DTOs/HelpTask/HelpTaskResponseDto.cs
using PawOfHelp.DTOs.Animal;
using PawOfHelp.DTOs.Public;

namespace PawOfHelp.DTOs.HelpTask;

public class HelpTaskResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short RequiredVolunteers { get; set; }
    public int CountResponses { get; set; }
    public bool IsTaskOverexposure { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public PublicProfileDto Creator { get; set; } = new();
    public List<AnimalShortResponseDto> Animals { get; set; } = new();
    public List<string> Competencies { get; set; } = new();
    public List<string> Locations { get; set; } = new();
    public List<PublicProfileDto> Workers { get; set; } = new();
}