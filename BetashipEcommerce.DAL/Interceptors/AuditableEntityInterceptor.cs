using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Interceptors
{
    /// <summary>
    /// Automatically populates audit fields (CreatedBy, UpdatedBy, etc.) on entities
    /// </summary>
    public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditableEntityInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateAuditableEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditableEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditableEntities(DbContext? context)
        {
            if (context == null) return;

            var userId = _currentUserService.UserId;
            var username = _currentUserService.Username ?? "System";

            foreach (var entry in context.ChangeTracker.Entries<AuditableEntity<object>>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (userId != null)
                            entry.Entity.SetCreatedBy(userId, username);
                        break;

                    case EntityState.Modified:
                        if (userId != null)
                            entry.Entity.SetUpdatedBy(userId, username);
                        break;

                    case EntityState.Deleted:
                        // Handle soft delete (converted to update)
                        if (entry.Entity is AuditableEntity<object> auditableEntity)
                        {
                            entry.State = EntityState.Modified;
                            if (userId != null)
                                auditableEntity.SetDeletedBy(userId, username);
                        }
                        break;
                }
            }
        }
    }

}
