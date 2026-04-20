// DTOs/Auth/OrganizationAuthResponseDto.cs
namespace PawOfHelp.DTOs.Auth;

public class OrganizationAuthResponseDto : VolunteerAuthResponseDto
{
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? DonationDetails { get; set; }
}