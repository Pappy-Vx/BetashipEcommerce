using BetashipEcommerce.APP.Common.Models;
using BetashipEcommerce.APP.Queries.Orders.GetOrderById;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Orders.GetOrderHistory;

public sealed record GetOrderHistoryQuery(
    DateTime? From,
    DateTime? To,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PaginatedList<OrderDto>>;
