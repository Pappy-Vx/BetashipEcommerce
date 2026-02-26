using MediatR;

namespace BetashipEcommerce.APP.Queries.Carts.GetCartSummary;

public sealed record GetCartSummaryQuery(Guid CustomerId) : IRequest<CartSummaryDto?>;
