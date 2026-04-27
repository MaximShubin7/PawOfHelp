// DTOs/User/UpdateProfileDto.cs
namespace PawOfHelp.DTOs.User;

public class UpdateProfileDto
{
    public string? Name { get; set; }
    public short? Age { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public List<string>? CompetencyNames { get; set; }
    public List<string>? PreferenceNames { get; set; }
    public List<string>? AvailabilityNames { get; set; }
    public IFormFile? Photo { get; set; }
    public string? PhotoUrlFromWeb { get; set; }
}