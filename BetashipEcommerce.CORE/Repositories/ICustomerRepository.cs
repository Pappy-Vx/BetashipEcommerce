using BetashipEcommerce.CORE.Customers;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Repositories
{
    public interface ICustomerRepository : IRepository<Customer, CustomerId>
    {
        Task<Customer?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<bool> IsEmailUniqueAsync(
            string email,
            CustomerId? excludeCustomerId = null,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Customer>> GetActiveCustomersAsync(
            CancellationToken cancellationToken = default);
    }

}
