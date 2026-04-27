// Validators/Post/CreatePostValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.Post;

namespace PawOfHelp.Validators.Post;

public class CreatePostValidator : AbstractValidator<CreatePostDto>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Заголовок обязателен")
            .MaximumLength(200).WithMessage("Заголовок не должен превышать 200 символов");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание обязательно")
            .MaximumLength(5000).WithMessage("Описание не должно превышать 5000 символов");
    }
}