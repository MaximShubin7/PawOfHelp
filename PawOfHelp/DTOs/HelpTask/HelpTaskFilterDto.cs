// DTOs/HelpTask/HelpTaskFilterDto.cs
namespace PawOfHelp.DTOs.HelpTask;

public class HelpTaskFilterDto
{
    public string? Search { get; set; }
    public List<string>? Locations { get; set; }
    public List<string>? Competencies { get; set; }
    public List<string>? Preferences { get; set; }
    public List<string>? Availabilities { get; set; }
    public bool? IsTaskOverexposure { get; set; }
    public DateTime? StartedAfter { get; set; }
    public DateTime? StartedBefore { get; set; }
}   