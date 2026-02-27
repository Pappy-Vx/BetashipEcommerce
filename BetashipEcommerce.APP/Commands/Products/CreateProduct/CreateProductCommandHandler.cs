using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.Enums;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Products.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Check for duplicate SKU
        var existingProduct = await _productRepository.FindOneAsync(
            p => p.Sku == request.Sku, cancellationToken);

        if (existingProduct != null)
            return Result.Failure<Guid>(ProductErrors.DuplicateSku);

        // Create Money value object
        var price = Money.Create(request.Price, request.Currency);

        // Create Product via factory method (domain logic)
        var productResult = Product.Create(
            request.Name,
            request.Description,
            request.Sku,
            price,
            (ProductCategory)request.Category);

        if (!productResult.IsSuccess)
            return Result.Failure<Guid>(productResult.Error);

        var product = productResult.Value;

        _productRepository.Add(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Product created: {ProductId} - {ProductName}",
            product.Id.Value, product.Name);

        return Result.Success(product.Id.Value);
    }
}
