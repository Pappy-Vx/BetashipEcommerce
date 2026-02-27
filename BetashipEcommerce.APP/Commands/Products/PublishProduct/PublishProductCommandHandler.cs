using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Products.PublishProduct;

public sealed class PublishProductCommandHandler : IRequestHandler<PublishProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublishProductCommandHandler> _logger;

    public PublishProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<PublishProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(PublishProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            new ProductId(request.ProductId), cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        var result = product.Publish();
        if (!result.IsSuccess)
            return result;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} published", request.ProductId);

        return Result.Success();
    }
}
