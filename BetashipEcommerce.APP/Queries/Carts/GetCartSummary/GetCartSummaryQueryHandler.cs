using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Services;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Carts.GetCartSummary;

public sealed class GetCartSummaryQueryHandler : IRequestHandler<GetCartSummaryQuery, CartSummaryDto?>
{
    private readonly ICartCacheService _cartCache;

    public GetCartSummaryQueryHandler(ICartCacheService cartCache)
    {
        _cartCache = cartCache;
    }

    public async Task<CartSummaryDto?> Handle(
        GetCartSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var cart = await _cartCache.GetCartAsync(new CustomerId(request.CustomerId), cancellationToken);

        if (cart is null)
            return null;

        return new CartSummaryDto(
            cart.CustomerId,
            cart.TotalItems,
            cart.Items.Count,
            cart.CreatedAt,
            cart.LastModifiedAt);
    }
}
