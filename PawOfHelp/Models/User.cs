// Models/User.cs
namespace PawOfHelp.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public short? Age { get; set; }
    public string? Description { get; set; }
    public int SumRating { get; set; }
    public int CountRating { get; set; }
    public short Role { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}