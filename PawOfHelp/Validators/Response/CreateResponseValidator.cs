// Validators/Response/CreateResponseValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.Response;

namespace PawOfHelp.Validators.Response;

public class CreateResponseValidator : AbstractValidator<CreateResponseDto>
{
    public CreateResponseValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("ID задачи обязателен");
    }
}