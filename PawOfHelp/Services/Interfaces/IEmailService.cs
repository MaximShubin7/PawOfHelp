// Services/Interfaces/IEmailService.cs
namespace PawOfHelp.Services.Interfaces;

public interface IEmailService
{
    Task SendVerificationCodeAsync(string email, string code);
    Task SendPasswordResetCodeAsync(string email, string code);
}