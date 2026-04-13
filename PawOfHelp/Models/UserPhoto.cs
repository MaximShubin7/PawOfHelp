// Models/UserPhoto.cs
namespace PawOfHelp.Models;

public class UserPhoto
{
    public Guid UserId { get; set; }
    public byte[] PhotoData { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}