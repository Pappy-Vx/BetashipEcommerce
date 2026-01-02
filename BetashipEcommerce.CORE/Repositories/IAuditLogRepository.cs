using BetashipEcommerce.CORE.Auditing;
using BetashipEcommerce.CORE.Auditing.Enums;
using BetashipEcommerce.CORE.Identity.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Repositories
{
    public interface IAuditLogRepository
    {
        Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

        Task<List<AuditLog>> GetByEntityAsync(
            string entityType,
            string entityId,
            CancellationToken cancellationToken = default);

        Task<List<AuditLog>> GetByUserAsync(
            UserId userId,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cancellationToken = default);

        Task<List<AuditLog>> GetRecentAsync(
            int count,
            CancellationToken cancellationToken = default);

        Task<List<AuditLog>> GetByActionAsync(
            AuditAction action,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cancellationToken = default);
    }
}
