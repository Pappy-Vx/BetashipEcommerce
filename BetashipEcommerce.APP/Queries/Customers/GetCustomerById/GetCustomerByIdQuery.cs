using MediatR;

namespace BetashipEcommerce.APP.Queries.Customers.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDto?>;
