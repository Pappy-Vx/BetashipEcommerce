using BetashipEcommerce.APP.Queries.Orders.GetOrderById;
using BetashipEcommerce.CORE.Customers;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Customers.GetCustomerOrders;

public sealed class GetCustomerOrdersQueryHandler
    : IRequestHandler<GetCustomerOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerOrdersQueryHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(
        GetCustomerOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            new CustomerId(request.CustomerId), cancellationToken);

        if (customer is null)
            return [];

        var orders = await _orderRepository.GetByCustomerIdAsync(
            customer.Id, cancellationToken);

        return orders.Select(order => new OrderDto(
            order.Id.Value,
            order.CustomerId.Value,
            order.OrderNumber,
            order.Status.ToString(),
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            order.OrderDate,
            order.ConfirmedAt,
            order.ShippedAt,
            order.DeliveredAt,
            order.CancelledAt,
            order.CancellationReason,
            order.IsPaid,
            order.Items.Select(i => new OrderItemDto(
                i.ProductId.Value,
                i.ProductName,
                i.Quantity,
                i.UnitPrice.Amount,
                i.TotalPrice.Amount,
                i.UnitPrice.Currency)).ToList()
        )).ToList();
    }
}
