// Validators/HelpTask/UpdateHelpTaskValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.HelpTask;

namespace PawOfHelp.Validators.HelpTask;

public class UpdateHelpTaskValidator : AbstractValidator<UpdateHelpTaskDto>
{
    public UpdateHelpTaskValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Заголовок не должен превышать 200 символов")
            .When(x => x.Title != null);

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Описание не должно превышать 5000 символов")
            .When(x => x.Description != null);

        RuleFor(x => x.RequiredVolunteers)
            .Must(x => x > 0).WithMessage("Количество волонтёров должно быть больше 0")
            .Must(x => x <= 100).WithMessage("Количество волонтёров не должно превышать 100")
            .When(x => x.RequiredVolunteers.HasValue);
    }
}