using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Orders.PlaceOrder;

public sealed class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PlaceOrderCommandHandler> _logger;

    public PlaceOrderCommandHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<PlaceOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        // Verify customer exists
        var customer = await _customerRepository.GetByIdAsync(
            new CustomerId(request.CustomerId), cancellationToken);

        if (customer is null)
            return Result.Failure<Guid>(new Error("Order.CustomerNotFound", "Customer not found."));

        // Fetch products and build order items
        var orderItems = new List<(ProductId ProductId, int Quantity, Money UnitPrice, string ProductName)>();

        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(
                new ProductId(item.ProductId), cancellationToken);

            if (product is null)
                return Result.Failure<Guid>(new Error(
                    "Order.ProductNotFound",
                    $"Product {item.ProductId} not found."));

            orderItems.Add((product.Id, item.Quantity, product.Price, product.Name));
        }

        var shippingAddress = Address.Create(
            request.ShippingStreet, request.ShippingCity,
            request.ShippingState, request.ShippingCountry,
            request.ShippingPostalCode);

        var billingAddress = Address.Create(
            request.BillingStreet, request.BillingCity,
            request.BillingState, request.BillingCountry,
            request.BillingPostalCode);

        var orderResult = Order.Create(
            new CustomerId(request.CustomerId),
            shippingAddress,
            billingAddress,
            orderItems);

        if (!orderResult.IsSuccess)
            return Result.Failure<Guid>(orderResult.Error);

        var order = orderResult.Value;

        _orderRepository.Add(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order {OrderId} placed for customer {CustomerId}. Items: {ItemCount}",
            order.Id.Value, request.CustomerId, request.Items.Count);

        return Result.Success(order.Id.Value);
    }
}
