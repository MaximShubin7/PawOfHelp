// Validators/Comment/CreateCommentValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.Comment;

namespace PawOfHelp.Validators.Comment;

public class CreateCommentValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Rating)
            .NotEmpty().WithMessage("Рейтинг обязателен")
            .Must(rating => rating >= 1 && rating <= 5).WithMessage("Рейтинг должен быть от 1 до 5");

        RuleFor(x => x.RecipientId)
            .NotEmpty().WithMessage("ID получателя обязателен");
    }
}