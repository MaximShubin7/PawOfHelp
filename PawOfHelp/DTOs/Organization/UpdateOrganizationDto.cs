// DTOs/Organization/UpdateOrganizationDto.cs
namespace PawOfHelp.DTOs.Organization;

public class UpdateOrganizationDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? DonationDetails { get; set; }
    public string? Description { get; set; }
    public IFormFile? Photo { get; set; }
    public string? PhotoUrlFromWeb { get; set; }
}