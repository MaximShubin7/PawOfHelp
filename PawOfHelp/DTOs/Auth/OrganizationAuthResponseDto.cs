// DTOs/Auth/OrganizationAuthResponseDto.cs
using PawOfHelp.DTOs.Post;

namespace PawOfHelp.DTOs.Auth;

public class OrganizationAuthResponseDto : AuthResponseDto
{
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? DonationDetails { get; set; }
    public PostResponseDto? LatestPost { get; set; }
    public List<string> ConstantNeeds { get; set; } = new();
}