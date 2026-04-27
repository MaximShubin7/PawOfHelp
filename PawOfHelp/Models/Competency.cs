// Models/Competency.cs
namespace PawOfHelp.Models;

public class Competency
{
    public short Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<UserCompetency> UserCompetencies { get; set; } = new List<UserCompetency>();
}