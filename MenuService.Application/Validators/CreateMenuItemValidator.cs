using FluentValidation;
using MenuService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            RuleFor(x => x.Emoji)
                .NotEmpty().WithMessage("Emoji is required.")
                .MaximumLength(10).WithMessage("Emoji cannot exceed 10 characters.");
            RuleFor(x => x.Badge)
                .MaximumLength(20).WithMessage("Badge cannot exceed 20 characters.");
        }
    }
}
