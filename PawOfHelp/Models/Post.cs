// Models/Post.cs
namespace PawOfHelp.Models;

public class Post
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public OrganizationDetails Organization { get; set; } = null!;
    public ICollection<PostPhoto> Photos { get; set; } = new List<PostPhoto>();
}