using BetashipEcommerce.CORE.Customers;
using BetashipEcommerce.CORE.Customers.Entities;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Customers.AddCustomerAddress;

public sealed class AddCustomerAddressCommandHandler : IRequestHandler<AddCustomerAddressCommand, Result<Guid>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddCustomerAddressCommandHandler> _logger;

    public AddCustomerAddressCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddCustomerAddressCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(AddCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            new CustomerId(request.CustomerId), cancellationToken);

        if (customer is null)
            return Result.Failure<Guid>(CustomerErrors.NotFound);

        var address = Address.Create(
            request.Street, request.City,
            request.State, request.Country,
            request.PostalCode);

        var customerAddress = CustomerAddress.Create(request.Label, address, request.IsDefault);

        var result = customer.AddAddress(customerAddress);
        if (!result.IsSuccess)
            return Result.Failure<Guid>(result.Error);

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Address added for customer {CustomerId}: {Label}",
            request.CustomerId, request.Label);

        return Result.Success(customerAddress.Id);
    }
}
