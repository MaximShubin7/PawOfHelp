// Models/VerificationCode.cs
namespace PawOfHelp.Models;

public class VerificationCode
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public short AttemptsLeft { get; set; }
    public short Role { get; set; }
    public DateTime CreatedAt { get; set; }
}