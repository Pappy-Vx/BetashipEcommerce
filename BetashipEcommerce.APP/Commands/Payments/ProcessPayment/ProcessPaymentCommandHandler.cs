using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Payments;
using BetashipEcommerce.CORE.Payments.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Payments.ProcessPayment;

public sealed class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(
            new PaymentId(request.PaymentId), cancellationToken);

        if (payment is null)
            return Result.Failure(PaymentErrors.NotFound);

        // Start processing
        var startResult = payment.StartProcessing();
        if (!startResult.IsSuccess)
            return startResult;

        // Complete payment
        var completeResult = payment.Complete(request.TransactionId, request.GatewayResponse);
        if (!completeResult.IsSuccess)
            return completeResult;

        // Mark order as paid
        var order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
        if (order is not null)
        {
            order.MarkAsPaid();
            _orderRepository.Update(order);
        }

        _paymentRepository.Update(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Payment {PaymentId} processed successfully. TransactionId: {TransactionId}",
            request.PaymentId, request.TransactionId);

        return Result.Success();
    }
}
