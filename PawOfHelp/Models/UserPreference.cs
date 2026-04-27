// Models/UserPreference.cs
namespace PawOfHelp.Models;

public class UserPreference
{
    public Guid UserId { get; set; }
    public short PreferenceId { get; set; }

    public User User { get; set; } = null!;
    public Preference Preference { get; set; } = null!;
}