// Models/Location.cs
namespace PawOfHelp.Models;

public class Location
{
    public short Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
}