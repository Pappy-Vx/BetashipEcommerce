using BetashipEcommerce.APP.Queries.Orders.GetOrderById;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Orders.GetOrdersByCustomer;

public sealed class GetOrdersByCustomerQueryHandler
    : IRequestHandler<GetOrdersByCustomerQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersByCustomerQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(
        GetOrdersByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(
            new CustomerId(request.CustomerId), cancellationToken);

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
