using FluentValidation;
using MenuService.Application.DTOs;

namespace MenuService.Application.Validators
{
    public class CreateMenuItemValidator : AbstractValidator<CreateMenuItemDto>
    {
        public CreateMenuItemValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(50).WithMessage("Category cannot exceed 50 characters.");

            RuleFor(x => x.Badge)
                .MaximumLength(20).WithMessage("Badge cannot exceed 20 characters.")
                .When(x => x.Badge is not null);

            // either ImageFile or ImageUrl must be provided
            RuleFor(x => x)
                .Must(x => x.ImageFile != null || !string.IsNullOrWhiteSpace(x.ImageUrl))
                .WithMessage("Image is required. Please upload a file or provide a URL.")
                .WithName("Image");
        }
    }
}
