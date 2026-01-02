using BetashipEcommerce.CORE.Identity;
using BetashipEcommerce.CORE.Identity.Enums;
using BetashipEcommerce.CORE.Identity.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Repositories
{
    public interface IUserRepository : IRepository<User, UserId>
    {
        Task<User?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default);

        Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<bool> IsUsernameUniqueAsync(
            string username,
            UserId? excludeUserId = null,
            CancellationToken cancellationToken = default);

        Task<bool> IsEmailUniqueAsync(
            string email,
            UserId? excludeUserId = null,
            CancellationToken cancellationToken = default);

        Task<List<User>> GetByRoleAsync(
            UserRole role,
            CancellationToken cancellationToken = default);

        Task<List<User>> GetActiveUsersAsync(
            CancellationToken cancellationToken = default);
    }
}
