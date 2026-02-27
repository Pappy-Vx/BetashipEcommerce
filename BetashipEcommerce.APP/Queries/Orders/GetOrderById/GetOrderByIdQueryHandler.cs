using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Orders.GetOrderById;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            new OrderId(request.OrderId), cancellationToken);

        if (order is null)
            return null;

        return new OrderDto(
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
                i.UnitPrice.Currency)).ToList());
    }
}
