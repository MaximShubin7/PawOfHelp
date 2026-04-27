// Models/Worker.cs (переименуй или создай новую)
namespace PawOfHelp.Models;

public class Worker
{
    public Guid UserId { get; set; }
    public Guid TaskId { get; set; }

    public User User { get; set; } = null!;
    public HelpTask HelpTask { get; set; } = null!;
}