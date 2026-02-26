using BetashipEcommerce.CORE.Orders;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Orders.ShipOrder;

public sealed class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, Result>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ShipOrderCommandHandler> _logger;

    public ShipOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<ShipOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            new OrderId(request.OrderId), cancellationToken);

        if (order == null)
            return Result.Failure(OrderErrors.NotFound);

        var result = order.Ship(request.TrackingNumber);
        if (!result.IsSuccess)
            return result;

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} shipped. Tracking: {Tracking}",
            request.OrderId, request.TrackingNumber ?? "N/A");

        return Result.Success();
    }
}
