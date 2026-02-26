using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Payments;
using BetashipEcommerce.CORE.Payments.Enums;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Payments.InitiatePayment;

public sealed class InitiatePaymentCommandHandler
    : IRequestHandler<InitiatePaymentCommand, Result<Guid>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InitiatePaymentCommandHandler> _logger;

    public InitiatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<InitiatePaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        InitiatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        // Verify order exists
        var order = await _orderRepository.GetByIdAsync(
            new OrderId(request.OrderId), cancellationToken);

        if (order == null)
            return Result.Failure<Guid>(new Error("Payment.OrderNotFound", "Order not found."));

        var amount = Money.Create(request.Amount, request.Currency);

        var paymentResult = Payment.Create(
            new OrderId(request.OrderId),
            new CustomerId(request.CustomerId),
            amount,
            (PaymentMethod)request.PaymentMethod);

        if (!paymentResult.IsSuccess)
            return Result.Failure<Guid>(paymentResult.Error);

        var payment = paymentResult.Value;

        // Link payment to order
        order.LinkPayment(payment.Id);

        _paymentRepository.Add(payment);
        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Payment {PaymentId} initiated for order {OrderId}. Amount: {Amount} {Currency}",
            payment.Id.Value, request.OrderId, request.Amount, request.Currency);

        return Result.Success(payment.Id.Value);
    }
}
