// Validators/Auth/RegisterRequestValidator.cs
using FluentValidation;
using System.Text.RegularExpressions;
using PawOfHelp.DTOs.Auth;

namespace PawOfHelp.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    private static readonly Regex PasswordRegex = new Regex(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
        RegexOptions.Compiled);

    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Неверный формат email")
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(8).WithMessage("Пароль должен быть минимум 8 символов")
            .Matches(PasswordRegex).WithMessage("Пароль должен содержать заглавные и строчные латинские буквы, цифры и спецсимвол");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Роль обязательна")
            .Must(role => role == 1 || role == 2)
            .WithMessage("Роль должна быть 1 (Volunteer) или 2 (Organization)");
    }
}