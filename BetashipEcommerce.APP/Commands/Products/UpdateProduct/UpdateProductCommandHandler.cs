using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Products.UpdateProduct;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            new ProductId(request.ProductId), cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        var isSkuUnique = await _productRepository.IsSkuUniqueAsync(
            request.Sku, new ProductId(request.ProductId), cancellationToken);

        if (!isSkuUnique)
            return Result.Failure(ProductErrors.DuplicateSku);

        var result = product.UpdateDetails(request.Name, request.Description, request.Sku);
        if (!result.IsSuccess)
            return result;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} updated", request.ProductId);

        return Result.Success();
    }
}
