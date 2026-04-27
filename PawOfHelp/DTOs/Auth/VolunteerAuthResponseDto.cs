// DTOs/Auth/VolunteerAuthResponseDto.cs
namespace PawOfHelp.DTOs.Auth;

public class VolunteerAuthResponseDto : AuthResponseDto
{
    public List<string> Competencies { get; set; } = new();
    public List<string> Preferences { get; set; } = new();
    public List<string> Availabilities { get; set; } = new();
}