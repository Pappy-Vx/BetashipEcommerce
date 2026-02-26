using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Orders.PlaceOrder;

public sealed record PlaceOrderCommand(
    Guid CustomerId,
    IReadOnlyList<PlaceOrderItemDto> Items,
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

public sealed record PlaceOrderItemDto(
    Guid ProductId,
    int Quantity);
