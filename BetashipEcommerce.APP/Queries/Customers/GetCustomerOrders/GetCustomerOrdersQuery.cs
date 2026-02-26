using BetashipEcommerce.APP.Queries.Orders.GetOrderById;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Customers.GetCustomerOrders;

public sealed record GetCustomerOrdersQuery(Guid CustomerId) : IRequest<IReadOnlyList<OrderDto>>;
