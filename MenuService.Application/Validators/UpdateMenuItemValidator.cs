using FluentValidation;
using MenuService.Application.DTOs;

namespace MenuService.Application.Validators
{
    public class UpdateMenuItemValidator : AbstractValidator<UpdateMenuItemDto>
    {
        public UpdateMenuItemValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100)
                .When(x => x.Name is not null);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => x.Description is not null);

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.")
                .When(x => x.Price is not null);

            RuleFor(x => x.Category)
                .MaximumLength(50)
                .When(x => x.Category is not null);
        }

    }
}
