using FluentValidation;

namespace BetashipEcommerce.APP.Commands.Orders.PlaceOrder;

public sealed class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
                .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100 per item.");
        });

        RuleFor(x => x.ShippingStreet).NotEmpty().WithMessage("Shipping street is required.");
        RuleFor(x => x.ShippingCity).NotEmpty().WithMessage("Shipping city is required.");
        RuleFor(x => x.ShippingCountry).NotEmpty().WithMessage("Shipping country is required.");

        RuleFor(x => x.BillingStreet).NotEmpty().WithMessage("Billing street is required.");
        RuleFor(x => x.BillingCity).NotEmpty().WithMessage("Billing city is required.");
        RuleFor(x => x.BillingCountry).NotEmpty().WithMessage("Billing country is required.");
    }
}
