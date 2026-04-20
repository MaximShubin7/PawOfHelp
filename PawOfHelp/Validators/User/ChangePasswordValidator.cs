// Validators/User/ChangePasswordValidator.cs
using FluentValidation;
using System.Text.RegularExpressions;
using PawOfHelp.DTOs.User;

namespace PawOfHelp.Validators.User;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
{
    private static readonly Regex PasswordRegex = new Regex(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
        RegexOptions.Compiled);

    public ChangePasswordValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Старый пароль обязателен");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Новый пароль обязателен")
            .MinimumLength(8).WithMessage("Пароль должен быть минимум 8 символов")
            .Matches(PasswordRegex).WithMessage("Пароль должен содержать заглавные и строчные латинские буквы, цифры и спецсимвол")
            .NotEqual(x => x.OldPassword).WithMessage("Новый пароль должен отличаться от старого");
    }
}