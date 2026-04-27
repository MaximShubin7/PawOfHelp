// Validators/Animal/UpdateAnimalValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.Animal;

namespace PawOfHelp.Validators.Animal;

public class UpdateAnimalValidator : AbstractValidator<UpdateAnimalDto>
{
    public UpdateAnimalValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов")
            .When(x => x.Name != null);

        RuleFor(x => x.Breed)
            .MaximumLength(100).WithMessage("Порода не должна превышать 100 символов")
            .When(x => x.Breed != null);

        RuleFor(x => x.Health)
            .MaximumLength(500).WithMessage("Здоровье не должно превышать 500 символов")
            .When(x => x.Health != null);

        RuleFor(x => x.Character)
            .MaximumLength(500).WithMessage("Характер не должен превышать 500 символов")
            .When(x => x.Character != null);

        RuleFor(x => x.SpecialNeeds)
            .MaximumLength(1000).WithMessage("Особые потребности не должны превышать 1000 символов")
            .When(x => x.SpecialNeeds != null);
    }
}