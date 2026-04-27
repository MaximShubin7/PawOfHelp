// Models/UserAvailability.cs
namespace PawOfHelp.Models;

public class UserAvailability
{
    public Guid UserId { get; set; }
    public short AvailabilityId { get; set; }

    public User User { get; set; } = null!;
    public Availability Availability { get; set; } = null!;
}