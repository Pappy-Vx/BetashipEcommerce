using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Customers.AddCustomerAddress;

public sealed record AddCustomerAddressCommand(
    Guid CustomerId,
    string Label,
    string Street,
    string City,
    string State,
    string Country,
    string PostalCode,
    bool IsDefault) : IRequest<Result<Guid>>;
