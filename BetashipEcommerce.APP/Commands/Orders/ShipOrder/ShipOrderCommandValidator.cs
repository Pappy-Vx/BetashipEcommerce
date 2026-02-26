using FluentValidation;

namespace BetashipEcommerce.APP.Commands.Orders.ShipOrder;

public sealed class ShipOrderCommandValidator : AbstractValidator<ShipOrderCommand>
{
    public ShipOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(x => x.TrackingNumber)
            .MaximumLength(100).WithMessage("Tracking number must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TrackingNumber));
    }
}
