namespace BetashipEcommerce.APP.Queries.Carts.GetCartSummary;

public sealed record CartSummaryDto(
    Guid CustomerId,
    int TotalItems,
    int UniqueProducts,
    DateTime CreatedAt,
    DateTime LastModifiedAt);
