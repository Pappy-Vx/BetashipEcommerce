namespace BetashipEcommerce.APP.Queries.Orders.GetOrderById;

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    string OrderNumber,
    string Status,
    decimal TotalAmount,
    string Currency,
    DateTime OrderDate,
    DateTime? ConfirmedAt,
    DateTime? ShippedAt,
    DateTime? DeliveredAt,
    DateTime? CancelledAt,
    string? CancellationReason,
    bool IsPaid,
    IReadOnlyList<OrderItemDto> Items);

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string Currency);
