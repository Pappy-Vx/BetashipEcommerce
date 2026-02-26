using MediatR;

namespace BetashipEcommerce.APP.Queries.Products.GetProductById;

public sealed record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDto?>;
