using BetashipEcommerce.APP.Queries.Orders.GetOrderById;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Orders.GetOrdersByCustomer;

public sealed record GetOrdersByCustomerQuery(Guid CustomerId) : IRequest<IReadOnlyList<OrderDto>>;
