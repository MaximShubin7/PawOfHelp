// Validators/Response/UpdateResponseStatusValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.Response;

namespace PawOfHelp.Validators.Response;

public class UpdateResponseStatusValidator : AbstractValidator<UpdateResponseStatusDto>
{
    public UpdateResponseStatusValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Статус обязателен")
            .Must(status => status == "На рассмотрении" || status == "Принят" || status == "Отклонен")
            .WithMessage("Статус должен быть: 'На рассмотрении', 'Принят' или 'Отклонен'");
    }
}