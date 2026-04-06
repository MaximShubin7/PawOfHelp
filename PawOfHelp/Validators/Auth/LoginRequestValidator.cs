using FluentValidation;
using PawOfHelp.DTOs.Auth;

namespace PawOfHelp.Validators.Auth
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255).WithMessage("Неверный формат email");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Пароль обязателен");
        }
    }
}
