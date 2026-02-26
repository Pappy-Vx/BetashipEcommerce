using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Products.GetProductById;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            new ProductId(request.ProductId), cancellationToken);

        if (product == null)
            return null;

        return new ProductDto(
            product.Id.Value,
            product.Name,
            product.Description,
            product.Sku,
            product.Price.Amount,
            product.Price.Currency,
            product.Category.ToString(),
            product.Status.ToString(),
            product.CreatedAt,
            product.UpdatedAt);
    }
}
