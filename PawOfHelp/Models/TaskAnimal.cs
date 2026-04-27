// Models/TaskAnimal.cs
namespace PawOfHelp.Models;

public class TaskAnimal
{
    public Guid TaskId { get; set; }
    public Guid AnimalId { get; set; }

    public HelpTask HelpTask { get; set; } = null!;
    public Animal Animal { get; set; } = null!;
}