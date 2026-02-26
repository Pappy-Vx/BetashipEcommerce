using BetashipEcommerce.CORE.SharedKernel;
using MediatR;

namespace BetashipEcommerce.APP.Commands.Customers.RegisterCustomer;

public sealed record RegisterCustomerCommand(
    string Email,
    string FirstName,
    string LastName) : IRequest<Result<Guid>>;
