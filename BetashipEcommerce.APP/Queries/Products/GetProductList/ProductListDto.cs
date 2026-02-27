namespace BetashipEcommerce.APP.Queries.Products.GetProductList;

public sealed record ProductListDto(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    string Currency,
    string Category,
    string Status);
