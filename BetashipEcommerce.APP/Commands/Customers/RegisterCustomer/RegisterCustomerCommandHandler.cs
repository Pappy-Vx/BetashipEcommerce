using BetashipEcommerce.CORE.Customers;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Customers.RegisterCustomer;

public sealed class RegisterCustomerCommandHandler
    : IRequestHandler<RegisterCustomerCommand, Result<Guid>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterCustomerCommandHandler> _logger;

    public RegisterCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<RegisterCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        RegisterCustomerCommand request,
        CancellationToken cancellationToken)
    {
        // Check for duplicate email
        var existing = await _customerRepository.FindOneAsync(
            c => c.Email.Value == request.Email.ToLowerInvariant(),
            cancellationToken);

        if (existing != null)
            return Result.Failure<Guid>(CustomerErrors.DuplicateEmail);

        var customerResult = Customer.Create(request.Email, request.FirstName, request.LastName);
        if (!customerResult.IsSuccess)
            return Result.Failure<Guid>(customerResult.Error);

        var customer = customerResult.Value;

        _customerRepository.Add(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer registered: {CustomerId} - {Email}",
            customer.Id.Value, request.Email);

        return Result.Success(customer.Id.Value);
    }
}
