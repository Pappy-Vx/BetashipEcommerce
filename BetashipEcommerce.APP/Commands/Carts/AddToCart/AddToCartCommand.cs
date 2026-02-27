using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Carts.AddToCart;

public sealed record AddToCartCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity) : IRequest<Result>;
