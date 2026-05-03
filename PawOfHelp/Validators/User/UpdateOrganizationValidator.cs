// Validators/Organization/UpdateOrganizationValidator.cs
using FluentValidation;
using PawOfHelp.DTOs.Organization;

namespace PawOfHelp.Validators.Organization;

public class UpdateOrganizationValidator : AbstractValidator<UpdateOrganizationDto>
{
    public UpdateOrganizationValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Название не должно превышать 100 символов")
            .When(x => x.Name != null);

        RuleFor(x => x.Phone)
            .Matches(@"^\+?\d{10,15}$").WithMessage("Телефон должен содержать 10-15 цифр, может начинаться с +")
            .When(x => x.Phone != null);

        RuleFor(x => x.Website)
            .MaximumLength(255).WithMessage("Сайт не должен превышать 255 символов")
            .When(x => x.Website != null);

        RuleFor(x => x.DonationDetails)
            .MaximumLength(500).WithMessage("Реквизиты не должны превышать 500 символов")
            .When(x => x.DonationDetails != null);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов")
            .When(x => x.Description != null);

        RuleFor(x => x.Location)
            .MaximumLength(255).WithMessage("Локация не должна превышать 255 символов")
            .When(x => x.Location != null);
    }
}