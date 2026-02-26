using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Carts.CheckoutCart;

/// <summary>
/// Checkout command: moves cart from Redis → Database, creates order.
/// This is the critical hybrid transition point.
/// </summary>
public sealed record CheckoutCartCommand(
    Guid CustomerId,
    string ShippingStreet,
    string ShippingCity,
    string ShippingState,
    string ShippingCountry,
    string ShippingPostalCode,
    string BillingStreet,
    string BillingCity,
    string BillingState,
    string BillingCountry,
    string BillingPostalCode) : IRequest<Result<Guid>>;
