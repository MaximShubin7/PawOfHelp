// Models/OrganizationDetails.cs
namespace PawOfHelp.Models;

public class OrganizationDetails
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? DonationDetails { get; set; }
    public User User { get; set; } = null!;
}