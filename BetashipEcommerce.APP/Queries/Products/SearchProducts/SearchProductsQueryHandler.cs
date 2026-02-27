using BetashipEcommerce.APP.Common.Models;
using BetashipEcommerce.CORE.Repositories;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Products.SearchProducts;

public sealed class SearchProductsQueryHandler
    : IRequestHandler<SearchProductsQuery, PaginatedList<ProductSearchResultDto>>
{
    private readonly IProductRepository _productRepository;

    public SearchProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PaginatedList<ProductSearchResultDto>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken)
    {
        var allProducts = await _productRepository.GetAllAsync(cancellationToken);

        var term = request.SearchTerm.Trim().ToLowerInvariant();

        var matched = allProducts
            .Where(p =>
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Sku.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var totalCount = matched.Count;

        var items = matched
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductSearchResultDto(
                p.Id.Value,
                p.Name,
                p.Description,
                p.Sku,
                p.Price.Amount,
                p.Price.Currency,
                p.Category.ToString(),
                p.Status.ToString()))
            .ToList();

        return new PaginatedList<ProductSearchResultDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
