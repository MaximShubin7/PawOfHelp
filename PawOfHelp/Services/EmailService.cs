// Services/EmailService.cs
using Resend;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class EmailService : IEmailService
{
    private readonly IResend _resend;

    public EmailService(IResend resend)
    {
        _resend = resend;
    }

    public async Task SendVerificationCodeAsync(string email, string code)
    {
        var message = new EmailMessage();
        message.From = "noreply@pawofhelp.abrdns.com";
        message.To.Add(email);
        message.Subject = "Код подтверждения регистрации";
        message.HtmlBody = $"<h1>Ваш код подтверждения: <strong>{code}</strong></h1><p>Код действителен 10 минут.</p>";

        await _resend.EmailSendAsync(message);
    }
}