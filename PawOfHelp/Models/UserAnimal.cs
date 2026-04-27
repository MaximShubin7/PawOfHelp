// Models/UserAnimal.cs
namespace PawOfHelp.Models;

public class UserAnimal
{
    public Guid UserId { get; set; }
    public Guid AnimalId { get; set; }

    public User User { get; set; } = null!;
    public Animal Animal { get; set; } = null!;
}