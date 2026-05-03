// Models/Response.cs
namespace PawOfHelp.Models;

public class Response
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid TaskId { get; set; }
    public short StatusId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User Sender { get; set; } = null!;
    public HelpTask HelpTask { get; set; } = null!;
    public Status Status { get; set; } = null!;
}