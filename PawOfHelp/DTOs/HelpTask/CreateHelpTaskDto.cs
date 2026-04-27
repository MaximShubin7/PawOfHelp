// DTOs/HelpTask/CreateHelpTaskDto.cs
namespace PawOfHelp.DTOs.HelpTask;

public class CreateHelpTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short RequiredVolunteers { get; set; } = 1;
    public bool IsTaskOverexposure { get; set; } = false;
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public List<Guid>? AnimalIds { get; set; }
    public List<string>? Competencies { get; set; }
    public List<string>? Locations { get; set; }
}