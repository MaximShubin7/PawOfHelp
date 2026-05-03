// Validators/User/UpdateProfileValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.User;

namespace PawOfHelp.Validators.User;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов")
            .When(x => x.Name != null);

        RuleFor(x => x.Age)
            .Must(age => age == null || (age >= 14 && age <= 120))
            .WithMessage("Возраст должен быть от 14 до 120 лет");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов")
            .When(x => x.Description != null);

        RuleFor(x => x.Location)
            .MaximumLength(255).WithMessage("Локация не должна превышать 255 символов")
            .When(x => x.Location != null);
    }
}