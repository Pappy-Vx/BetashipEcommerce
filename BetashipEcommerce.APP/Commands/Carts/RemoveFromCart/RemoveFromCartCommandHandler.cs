using BetashipEcommerce.CORE.Carts;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Services;
using BetashipEcommerce.CORE.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Carts.RemoveFromCart;

public sealed class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, Result>
{
    private readonly ICartCacheService _cartCache;
    private readonly ILogger<RemoveFromCartCommandHandler> _logger;

    public RemoveFromCartCommandHandler(
        ICartCacheService cartCache,
        ILogger<RemoveFromCartCommandHandler> logger)
    {
        _cartCache = cartCache;
        _logger = logger;
    }

    public async Task<Result> Handle(
        RemoveFromCartCommand request,
        CancellationToken cancellationToken)
    {
        var removed = await _cartCache.RemoveItemAsync(
            new CustomerId(request.CustomerId),
            new ProductId(request.ProductId),
            cancellationToken);

        if (!removed)
            return Result.Failure(CartErrors.ItemNotFound);

        _logger.LogInformation(
            "Removed product {ProductId} from cart for customer {CustomerId}",
            request.ProductId, request.CustomerId);

        return Result.Success();
    }
}
