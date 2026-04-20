// Validators/Auth/ResetPasswordValidator.cs
using FluentValidation;
using System.Text.RegularExpressions;
using PawOfHelp.DTOs.Auth;

namespace PawOfHelp.Validators.Auth;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordWithCodeRequestDto>
{
    private static readonly Regex PasswordRegex = new Regex(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
        RegexOptions.Compiled);

    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Неверный формат email");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Код обязателен")
            .Length(6).WithMessage("Код должен содержать 6 символов");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Новый пароль обязателен")
            .MinimumLength(8).WithMessage("Пароль должен быть минимум 8 символов")
            .Matches(PasswordRegex).WithMessage("Пароль должен содержать заглавные и строчные латинские буквы, цифры и спецсимвол");
    }
}