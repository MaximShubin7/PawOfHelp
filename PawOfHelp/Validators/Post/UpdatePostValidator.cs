// Validators/Post/UpdatePostValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.Post;

namespace PawOfHelp.Validators.Post;

public class UpdatePostValidator : AbstractValidator<UpdatePostDto>
{
    public UpdatePostValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Заголовок не должен превышать 200 символов")
            .When(x => x.Title != null);

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Описание не должно превышать 5000 символов")
            .When(x => x.Description != null);
    }
}