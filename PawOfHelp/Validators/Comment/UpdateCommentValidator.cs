// Validators/Comment/UpdateCommentValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.Comment;

namespace PawOfHelp.Validators.Comment;

public class UpdateCommentValidator : AbstractValidator<UpdateCommentDto>
{
    public UpdateCommentValidator()
    {
        RuleFor(x => x.Rating)
            .Must(rating => !rating.HasValue || (rating.Value >= 1 && rating.Value <= 5))
            .WithMessage("Рейтинг должен быть от 1 до 5");
    }
}