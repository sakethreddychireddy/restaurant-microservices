using FluentValidation;
using OrderService.Application.DTOs;

namespace OrderService.Application.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.DeliveryAddress)
                 .NotEmpty().WithMessage("Delivery address is required.")
                 .MaximumLength(300);

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must have at least one item.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.MenuItemId)
                    .NotEmpty().WithMessage("MenuItemId is required.");
                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be at least 1.");
            });
        }
    }
}
