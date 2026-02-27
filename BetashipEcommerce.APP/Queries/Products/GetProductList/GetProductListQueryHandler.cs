using BetashipEcommerce.APP.Common.Models;
using BetashipEcommerce.CORE.Products.Enums;
using BetashipEcommerce.CORE.Repositories;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Products.GetProductList;

public sealed class GetProductListQueryHandler
    : IRequestHandler<GetProductListQuery, PaginatedList<ProductListDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductListQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PaginatedList<ProductListDto>> Handle(
        GetProductListQuery request,
        CancellationToken cancellationToken)
    {
        var allProducts = await _productRepository.GetAllAsync(cancellationToken);

        // Apply filters
        var filtered = allProducts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLowerInvariant();
            filtered = filtered.Where(p =>
                p.Name.ToLowerInvariant().Contains(term) ||
                p.Sku.ToLowerInvariant().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.CategoryFilter) &&
            System.Enum.TryParse<ProductCategory>(request.CategoryFilter, true, out var category))
        {
            filtered = filtered.Where(p => p.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(request.StatusFilter) &&
            System.Enum.TryParse<ProductStatus>(request.StatusFilter, true, out var status))
        {
            filtered = filtered.Where(p => p.Status == status);
        }

        var totalCount = filtered.Count();

        var items = filtered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductListDto(
                p.Id.Value,
                p.Name,
                p.Sku,
                p.Price.Amount,
                p.Price.Currency,
                p.Category.ToString(),
                p.Status.ToString()))
            .ToList();

        return new PaginatedList<ProductListDto>(
            items, totalCount, request.PageNumber, request.PageSize);
    }
}
