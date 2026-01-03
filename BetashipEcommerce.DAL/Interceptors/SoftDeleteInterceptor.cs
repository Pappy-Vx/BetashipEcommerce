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
    /// Converts hard deletes to soft deletes for auditable entities
    /// </summary>
    public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ApplySoftDelete(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Deleted)
                {
                    // Check if entity has IsDeleted property
                    var isDeletedProperty = entry.Entity.GetType().GetProperty("IsDeleted");
                    if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool))
                    {
                        entry.State = EntityState.Modified;
                        isDeletedProperty.SetValue(entry.Entity, true);
                    }
                }
            }
        }
    }

}
