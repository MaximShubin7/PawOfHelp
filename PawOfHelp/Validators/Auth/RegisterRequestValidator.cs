using FluentValidation;
using PawOfHelp.DTOs.Auth;
using System.Text.RegularExpressions;

namespace PawOfHelp.Validators.Auth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        private static readonly Regex PasswordRegex = new Regex(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
            RegexOptions.Compiled);

        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255)
                .WithMessage("Неверный формат email");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(255)
                .Matches(PasswordRegex)
                .WithMessage("Неверный формат пароля");

            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(role => role == "Volunteer" || role == "Organization")
                .WithMessage("Неверный формат роли");
        }
    }
}
