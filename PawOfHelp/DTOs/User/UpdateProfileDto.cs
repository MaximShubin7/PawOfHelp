// DTOs/User/UpdateProfileDto.cs
namespace PawOfHelp.DTOs.User;

public class UpdateProfileDto
{
    public string? Name { get; set; }
    public short? Age { get; set; }
    public string? Description { get; set; }
    public IFormFile? Photo { get; set; }
    public string? PhotoUrlFromWeb { get; set; }
}