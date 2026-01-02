using BetashipEcommerce.CORE.Auditing.Enums;
using BetashipEcommerce.CORE.Identity.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Auditing
{
    public interface IAuditService
    {
        /// <summary>
        /// Create audit log entry
        /// </summary>
        Task LogAsync(
            UserId? userId,
            string username,
            AuditAction action,
            string entityType,
            string entityId,
            string? entityName = null,
            object? oldValues = null,
            object? newValues = null,
            List<string>? changedProperties = null,
            string? ipAddress = null,
            string? userAgent = null,
            string? additionalInfo = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get audit logs for specific entity
        /// </summary>
        Task<List<AuditLog>> GetEntityAuditHistoryAsync(
            string entityType,
            string entityId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get audit logs for specific user
        /// </summary>
        Task<List<AuditLog>> GetUserAuditHistoryAsync(
            UserId userId,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get recent audit logs
        /// </summary>
        Task<List<AuditLog>> GetRecentLogsAsync(
            int count = 100,
            CancellationToken cancellationToken = default);
    }

}
