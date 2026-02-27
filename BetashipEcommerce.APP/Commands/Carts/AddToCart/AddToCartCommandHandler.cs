using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.Services;
using BetashipEcommerce.CORE.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Carts.AddToCart;

/// <summary>
/// Adds item to Redis cart (blazing fast ~1ms).
/// Validates product exists before adding.
/// </summary>
public sealed class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result>
{
    private readonly ICartCacheService _cartCache;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<AddToCartCommandHandler> _logger;

    public AddToCartCommandHandler(
        ICartCacheService cartCache,
        IProductRepository productRepository,
        ILogger<AddToCartCommandHandler> logger)
    {
        _cartCache = cartCache;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(
        AddToCartCommand request,
        CancellationToken cancellationToken)
    {
        // Validate product exists
        var product = await _productRepository.GetByIdAsync(
            new ProductId(request.ProductId), cancellationToken);

        if (product == null)
            return Result.Failure(new Error("Cart.ProductNotFound", "Product not found."));

        // Add to Redis (fast path)
        await _cartCache.AddItemAsync(
            new CustomerId(request.CustomerId),
            new ProductId(request.ProductId),
            request.Quantity,
            cancellationToken);

        _logger.LogInformation(
            "Added {Qty}x product {ProductId} to Redis cart for customer {CustomerId}",
            request.Quantity, request.ProductId, request.CustomerId);

        return Result.Success();
    }
}
