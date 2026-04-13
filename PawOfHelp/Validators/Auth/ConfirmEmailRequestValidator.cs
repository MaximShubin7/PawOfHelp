// Validators/Auth/ConfirmEmailRequestValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.Auth;

namespace PawOfHelp.Validators.Auth;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequestDto>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Неверный формат email")
            .MaximumLength(255);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Код обязателен")
            .Length(6).WithMessage("Код должен содержать 6 символов");
    }
}