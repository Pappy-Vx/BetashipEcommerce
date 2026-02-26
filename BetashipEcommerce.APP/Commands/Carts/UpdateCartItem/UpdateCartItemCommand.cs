using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Carts.UpdateCartItem;

public sealed record UpdateCartItemCommand(
    Guid CustomerId,
    Guid ProductId,
    int NewQuantity) : IRequest<Result>;
