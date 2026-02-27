namespace BetashipEcommerce.APP.Queries.Products.GetProductById;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    string Currency,
    string Category,
    string Status,
    DateTime CreatedAt,
    DateTime? LastModifiedAt);
