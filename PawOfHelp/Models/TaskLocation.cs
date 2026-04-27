// Models/TaskLocation.cs
namespace PawOfHelp.Models;

public class TaskLocation
{
    public Guid TaskId { get; set; }
    public short LocationId { get; set; }

    public HelpTask HelpTask { get; set; } = null!;
    public Location Location { get; set; } = null!;
}