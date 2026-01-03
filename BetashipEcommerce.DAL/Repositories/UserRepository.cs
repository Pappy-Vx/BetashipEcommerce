using BetashipEcommerce.CORE.Identity;
using BetashipEcommerce.CORE.Identity.Enums;
using BetashipEcommerce.CORE.Identity.ValueObjects;
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
    internal sealed class UserRepository : Repository<User, UserId>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(u => u.Username == username.ToLowerInvariant(), cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(u => u.Email.Value == email.ToLowerInvariant(), cancellationToken);
        }

        public async Task<bool> IsUsernameUniqueAsync(
            string username,
            UserId? excludeUserId = null,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.Where(u => u.Username == username.ToLowerInvariant());

            if (excludeUserId != null)
            {
                query = query.Where(u => u.Id != excludeUserId);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> IsEmailUniqueAsync(
            string email,
            UserId? excludeUserId = null,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.Where(u => u.Email.Value == email.ToLowerInvariant());

            if (excludeUserId != null)
            {
                query = query.Where(u => u.Id != excludeUserId);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<List<User>> GetByRoleAsync(
            UserRole role,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(u => u.Roles.Contains(role))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<User>> GetActiveUsersAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(u => u.Status == UserStatus.Active)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }

}
