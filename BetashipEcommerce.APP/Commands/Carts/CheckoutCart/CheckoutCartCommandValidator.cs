using FluentValidation;

namespace BetashipEcommerce.APP.Commands.Carts.CheckoutCart;

public sealed class CheckoutCartCommandValidator : AbstractValidator<CheckoutCartCommand>
{
    public CheckoutCartCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.ShippingStreet).NotEmpty().WithMessage("Shipping street is required.");
        RuleFor(x => x.ShippingCity).NotEmpty().WithMessage("Shipping city is required.");
        RuleFor(x => x.ShippingCountry).NotEmpty().WithMessage("Shipping country is required.");

        RuleFor(x => x.BillingStreet).NotEmpty().WithMessage("Billing street is required.");
        RuleFor(x => x.BillingCity).NotEmpty().WithMessage("Billing city is required.");
        RuleFor(x => x.BillingCountry).NotEmpty().WithMessage("Billing country is required.");
    }
}
