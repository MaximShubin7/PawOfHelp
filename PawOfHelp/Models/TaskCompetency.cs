// Models/TaskCompetency.cs
namespace PawOfHelp.Models;

public class TaskCompetency
{
    public Guid TaskId { get; set; }
    public short CompetencyId { get; set; }

    public HelpTask HelpTask { get; set; } = null!;
    public Competency Competency { get; set; } = null!;
}