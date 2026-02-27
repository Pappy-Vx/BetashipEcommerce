using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Products.DiscontinueProduct;

public sealed class DiscontinueProductCommandHandler
    : IRequestHandler<DiscontinueProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DiscontinueProductCommandHandler> _logger;

    public DiscontinueProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<DiscontinueProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        DiscontinueProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            new ProductId(request.ProductId), cancellationToken);

        if (product == null)
            return Result.Failure(ProductErrors.NotFound);

        var result = product.Discontinue(request.Reason);
        if (!result.IsSuccess)
            return result;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Product {ProductId} discontinued. Reason: {Reason}",
            request.ProductId, request.Reason);

        return Result.Success();
    }
}
