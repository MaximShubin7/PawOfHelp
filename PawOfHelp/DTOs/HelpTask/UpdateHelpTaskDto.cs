// DTOs/HelpTask/UpdateHelpTaskDto.cs
namespace PawOfHelp.DTOs.HelpTask;

public class UpdateHelpTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public short? RequiredVolunteers { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public List<Guid>? AnimalIds { get; set; }
    public List<string>? Competencies { get; set; }
    public List<string>? Locations { get; set; }
}