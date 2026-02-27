using BetashipEcommerce.CORE.Services;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Carts.GetCart;

public sealed record GetCartQuery(Guid CustomerId) : IRequest<CartCacheDto?>;
