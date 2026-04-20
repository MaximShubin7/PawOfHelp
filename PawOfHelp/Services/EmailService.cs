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

    public async Task SendPasswordResetCodeAsync(string email, string code)
    {
        var message = new EmailMessage();
        message.From = "noreply@pawofhelp.abrdns.com";
        message.To.Add(email);
        message.Subject = "Код восстановления пароля";
        message.HtmlBody = $@"
            <h1>Восстановление пароля</h1>
            <p>Ваш код для восстановления пароля: <strong>{code}</strong></p>
            <p>Код действителен в течение 10 минут.</p>
            <p>Если вы не запрашивали восстановление пароля, просто проигнорируйте это письмо.</p>
        ";
        await _resend.EmailSendAsync(message);
    }
}