using MediatR;

namespace BetashipEcommerce.APP.Queries.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;
