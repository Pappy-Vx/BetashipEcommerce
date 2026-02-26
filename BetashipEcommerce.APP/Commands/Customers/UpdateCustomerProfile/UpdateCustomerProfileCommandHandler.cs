using BetashipEcommerce.CORE.Customers;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Commands.Customers.UpdateCustomerProfile;

public sealed class UpdateCustomerProfileCommandHandler : IRequestHandler<UpdateCustomerProfileCommand, Result>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCustomerProfileCommandHandler> _logger;

    public UpdateCustomerProfileCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCustomerProfileCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            new CustomerId(request.CustomerId), cancellationToken);

        if (customer is null)
            return Result.Failure(CustomerErrors.NotFound);

        var result = customer.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber);
        if (!result.IsSuccess)
            return result;

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer profile updated: {CustomerId}", request.CustomerId);

        return Result.Success();
    }
}
