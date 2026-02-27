using BetashipEcommerce.CORE.Carts;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.Services;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Carts.CheckoutCart;

/// <summary>
/// Handles checkout: reads cart from Redis, fetches current prices from DB,
/// creates an Order aggregate, persists to DB, removes cart from Redis.
/// 
/// Flow: Redis Cart → Fetch Prices → Create Order → DB → Remove Redis Cart
/// </summary>
public sealed class CheckoutCartCommandHandler
    : IRequestHandler<CheckoutCartCommand, Result<Guid>>
{
    private readonly ICartCacheService _cartCache;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CheckoutCartCommandHandler> _logger;

    public CheckoutCartCommandHandler(
        ICartCacheService cartCache,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<CheckoutCartCommandHandler> logger)
    {
        _cartCache = cartCache;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CheckoutCartCommand request,
        CancellationToken cancellationToken)
    {
        var customerId = new CustomerId(request.CustomerId);

        // 1. Get cart from Redis
        var cachedCart = await _cartCache.GetCartAsync(customerId, cancellationToken);
        if (cachedCart == null || cachedCart.Items.Count == 0)
            return Result.Failure<Guid>(CartErrors.EmptyCart);

        // 2. Fetch current prices for all products
        var orderItems = new List<(ProductId ProductId, int Quantity, Money UnitPrice, string ProductName)>();

        foreach (var item in cachedCart.Items)
        {
            var product = await _productRepository.GetByIdAsync(
                new ProductId(item.ProductId), cancellationToken);

            if (product == null)
                return Result.Failure<Guid>(new Error(
                    "Checkout.ProductNotFound",
                    $"Product {item.ProductId} is no longer available."));

            orderItems.Add((
                product.Id,
                item.Quantity,
                product.Price,
                product.Name));
        }

        // 3. Create addresses
        var shippingAddress = Address.Create(
            request.ShippingStreet, request.ShippingCity,
            request.ShippingState, request.ShippingCountry,
            request.ShippingPostalCode);

        var billingAddress = Address.Create(
            request.BillingStreet, request.BillingCity,
            request.BillingState, request.BillingCountry,
            request.BillingPostalCode);

        // 4. Create Order via domain factory
        var orderResult = Order.Create(
            customerId,
            shippingAddress,
            billingAddress,
            orderItems);

        if (!orderResult.IsSuccess)
            return Result.Failure<Guid>(orderResult.Error);

        var order = orderResult.Value;

        // 5. Persist to Database
        _orderRepository.Add(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Remove cart from Redis (successful checkout)
        await _cartCache.DeleteCartAsync(customerId, cancellationToken);

        _logger.LogInformation(
            "🛒→📦 Checkout complete! Order {OrderId} created for customer {CustomerId}. Items: {ItemCount}",
            order.Id.Value, request.CustomerId, cachedCart.Items.Count);

        return Result.Success(order.Id.Value);
    }
}
