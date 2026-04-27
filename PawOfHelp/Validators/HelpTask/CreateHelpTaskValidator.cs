// Validators/HelpTask/CreateHelpTaskValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.HelpTask;

namespace PawOfHelp.Validators.HelpTask;

public class CreateHelpTaskValidator : AbstractValidator<CreateHelpTaskDto>
{
    public CreateHelpTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Заголовок обязателен")
            .MaximumLength(200).WithMessage("Заголовок не должен превышать 200 символов");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Описание не должно превышать 5000 символов")
            .When(x => x.Description != null);

        RuleFor(x => x.RequiredVolunteers)
            .Must(x => x > 0).WithMessage("Количество волонтёров должно быть больше 0")
            .Must(x => x <= 100).WithMessage("Количество волонтёров не должно превышать 100");

        RuleFor(x => x.StartedAt)
            .NotEmpty().WithMessage("Дата начала обязательна");

        RuleFor(x => x.EndedAt)
            .NotEmpty().WithMessage("Дата окончания обязательна")
            .GreaterThan(x => x.StartedAt).WithMessage("Дата окончания должна быть позже даты начала");
    }
}