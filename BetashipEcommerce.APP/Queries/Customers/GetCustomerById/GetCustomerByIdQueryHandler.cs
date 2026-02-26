using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using MediatR;

namespace BetashipEcommerce.APP.Queries.Customers.GetCustomerById;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto?> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            new CustomerId(request.CustomerId), cancellationToken);

        if (customer is null)
            return null;

        return new CustomerDto(
            customer.Id.Value,
            customer.Email.Value,
            customer.FirstName,
            customer.LastName,
            customer.PhoneNumber?.Value,
            customer.Status.ToString(),
            customer.CreatedAt,
            customer.Addresses.Select(a => new CustomerAddressDto(
                a.Id,
                a.Label,
                a.Address.Street,
                a.Address.City,
                a.Address.State,
                a.Address.Country,
                a.Address.PostalCode,
                a.IsDefault)).ToList());
    }
}
