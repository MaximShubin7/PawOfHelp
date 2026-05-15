// Validators/ReferenceBook/ReferenceBookRequestValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.ReferenceBook;

namespace PawOfHelp.Validators.ReferenceBook;

public class ReferenceBookRequestValidator : AbstractValidator<ReferenceBookRequestDto>
{
    public ReferenceBookRequestValidator()
    {
        RuleFor(x => x.AnimalType)
            .NotEmpty().WithMessage("Тип животного обязателен")
            .MaximumLength(50).WithMessage("Тип животного не должен превышать 50 символов");
    }
}