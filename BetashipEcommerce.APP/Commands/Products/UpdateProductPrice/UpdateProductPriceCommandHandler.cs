using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Products.UpdateProductPrice;

public sealed class UpdateProductPriceCommandHandler
    : IRequestHandler<UpdateProductPriceCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductPriceCommandHandler> _logger;

    public UpdateProductPriceCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateProductPriceCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UpdateProductPriceCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            new ProductId(request.ProductId), cancellationToken);

        if (product == null)
            return Result.Failure(ProductErrors.NotFound);

        var newPrice = Money.Create(request.NewPrice, request.Currency);
        var result = product.UpdatePrice(newPrice);

        if (!result.IsSuccess)
            return result;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Product {ProductId} price updated to {Price} {Currency}",
            request.ProductId, request.NewPrice, request.Currency);

        return Result.Success();
    }
}
