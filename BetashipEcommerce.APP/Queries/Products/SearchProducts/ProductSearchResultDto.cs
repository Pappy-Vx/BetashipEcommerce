namespace BetashipEcommerce.APP.Queries.Products.SearchProducts;

public sealed record ProductSearchResultDto(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    string Currency,
    string Category,
    string Status);
