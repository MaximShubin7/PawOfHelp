// Models/PostPhoto.cs
namespace PawOfHelp.Models;

public class PostPhoto
{
    public Guid PostId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;

    public Post Post { get; set; } = null!;
}