using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Carts.ClearCart;

public sealed record ClearCartCommand(Guid CustomerId) : IRequest<Result>;
