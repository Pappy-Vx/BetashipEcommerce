using BetashipEcommerce.CORE.Carts;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Services;
using BetashipEcommerce.CORE.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Carts.UpdateCartItem;

public sealed class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result>
{
    private readonly ICartCacheService _cartCache;
    private readonly ILogger<UpdateCartItemCommandHandler> _logger;

    public UpdateCartItemCommandHandler(
        ICartCacheService cartCache,
        ILogger<UpdateCartItemCommandHandler> logger)
    {
        _cartCache = cartCache;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var customerId = new CustomerId(request.CustomerId);
        var productId = new ProductId(request.ProductId);

        var cartExists = await _cartCache.CartExistsAsync(customerId, cancellationToken);
        if (!cartExists)
            return Result.Failure(CartErrors.NotFound);

        var updated = await _cartCache.UpdateItemQuantityAsync(
            customerId, productId, request.NewQuantity, cancellationToken);

        if (!updated)
            return Result.Failure(CartErrors.ItemNotFound);

        _logger.LogInformation(
            "Cart item updated: customer {CustomerId}, product {ProductId}, new quantity {Quantity}",
            request.CustomerId, request.ProductId, request.NewQuantity);

        return Result.Success();
    }
}
