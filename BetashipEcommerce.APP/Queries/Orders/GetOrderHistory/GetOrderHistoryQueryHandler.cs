using BetashipEcommerce.APP.Common.Models;
using BetashipEcommerce.APP.Queries.Orders.GetOrderById;
using BetashipEcommerce.CORE.Repositories;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Orders.GetOrderHistory;

public sealed class GetOrderHistoryQueryHandler
    : IRequestHandler<GetOrderHistoryQuery, PaginatedList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderHistoryQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PaginatedList<OrderDto>> Handle(
        GetOrderHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var from = request.From ?? DateTime.UtcNow.AddYears(-1);
        var to = request.To ?? DateTime.UtcNow;

        var orders = await _orderRepository.GetOrdersForDateRangeAsync(from, to, cancellationToken);

        var dtos = orders.Select(order => new OrderDto(
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

        var totalCount = dtos.Count;
        var items = dtos
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PaginatedList<OrderDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
