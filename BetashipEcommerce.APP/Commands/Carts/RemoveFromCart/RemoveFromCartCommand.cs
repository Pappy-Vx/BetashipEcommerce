using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Carts.RemoveFromCart;

public sealed record RemoveFromCartCommand(
    Guid CustomerId,
    Guid ProductId) : IRequest<Result>;
