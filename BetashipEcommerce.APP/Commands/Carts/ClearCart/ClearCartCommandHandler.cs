using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Services;
using BetashipEcommerce.CORE.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Carts.ClearCart;

public sealed class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result>
{
    private readonly ICartCacheService _cartCacheService;
    private readonly ILogger<ClearCartCommandHandler> _logger;

    public ClearCartCommandHandler(
        ICartCacheService cartCacheService,
        ILogger<ClearCartCommandHandler> logger)
    {
        _cartCacheService = cartCacheService;
        _logger = logger;
    }

    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var customerId = new CustomerId(request.CustomerId);

        await _cartCacheService.ClearCartAsync(customerId, cancellationToken);

        _logger.LogInformation("Cart cleared for customer {CustomerId}", request.CustomerId);

        return Result.Success();
    }
}
