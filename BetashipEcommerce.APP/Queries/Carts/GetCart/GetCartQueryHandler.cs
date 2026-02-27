using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Services;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Carts.GetCart;

/// <summary>
/// Gets cart from Redis (~0.5ms) — blazing fast.
/// </summary>
public sealed class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartCacheDto?>
{
    private readonly ICartCacheService _cartCache;

    public GetCartQueryHandler(ICartCacheService cartCache)
    {
        _cartCache = cartCache;
    }

    public async Task<CartCacheDto?> Handle(
        GetCartQuery request,
        CancellationToken cancellationToken)
    {
        return await _cartCache.GetCartAsync(
            new CustomerId(request.CustomerId),
            cancellationToken);
    }
}
