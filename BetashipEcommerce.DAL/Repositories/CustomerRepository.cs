using BetashipEcommerce.CORE.Customers;
using BetashipEcommerce.CORE.Customers.Enums;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Repositories
{
    internal sealed class CustomerRepository : Repository<Customer, CustomerId>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Customer?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Email.Value == email.ToLowerInvariant(), cancellationToken);
        }

        public async Task<bool> IsEmailUniqueAsync(
            string email,
            CustomerId? excludeCustomerId = null,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.Where(c => c.Email.Value == email.ToLowerInvariant());

            if (excludeCustomerId != null)
            {
                query = query.Where(c => c.Id != excludeCustomerId);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Customer>> GetActiveCustomersAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(c => c.Status == CustomerStatus.Active)
                .Include(c => c.Addresses)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }

}
