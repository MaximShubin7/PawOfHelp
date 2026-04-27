// Models/HelpTask.cs
namespace PawOfHelp.Models;

public class HelpTask
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short RequiredVolunteers { get; set; }
    public int CountResponses { get; set; }
    public bool IsTaskOverexposure { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public User Creator { get; set; } = null!;
}