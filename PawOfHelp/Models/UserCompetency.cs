// Models/UserCompetency.cs
namespace PawOfHelp.Models;

public class UserCompetency
{
    public Guid UserId { get; set; }
    public short CompetencyId { get; set; }

    public User User { get; set; } = null!;
    public Competency Competency { get; set; } = null!;
}