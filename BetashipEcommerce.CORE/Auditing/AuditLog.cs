using BetashipEcommerce.CORE.Auditing.Enums;
using BetashipEcommerce.CORE.Auditing.ValueObjects;
using BetashipEcommerce.CORE.Identity.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Auditing
{
    /// <summary>
    /// Immutable audit log entry for tracking all system changes
    /// </summary>
    public sealed class AuditLog : Entity<AuditLogId>
    {
        public UserId? UserId { get; private set; }
        public string Username { get; private set; }
        public AuditAction Action { get; private set; }
        public string EntityType { get; private set; }
        public string EntityId { get; private set; }
        public string? EntityName { get; private set; }
        public string? OldValues { get; private set; }
        public string? NewValues { get; private set; }
        public string? ChangedProperties { get; private set; }
        public string IpAddress { get; private set; }
        public string UserAgent { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string? AdditionalInfo { get; private set; }

        private AuditLog(
            AuditLogId id,
            UserId? userId,
            string username,
            AuditAction action,
            string entityType,
            string entityId,
            string? entityName,
            string? oldValues,
            string? newValues,
            string? changedProperties,
            string ipAddress,
            string userAgent,
            string? additionalInfo) : base(id)
        {
            UserId = userId;
            Username = username;
            Action = action;
            EntityType = entityType;
            EntityId = entityId;
            EntityName = entityName;
            OldValues = oldValues;
            NewValues = newValues;
            ChangedProperties = changedProperties;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            Timestamp = DateTime.UtcNow;
            AdditionalInfo = additionalInfo;
        }

        private AuditLog() : base() { }

        public static AuditLog Create(
            UserId? userId,
            string username,
            AuditAction action,
            string entityType,
            string entityId,
            string? entityName = null,
            string? oldValues = null,
            string? newValues = null,
            string? changedProperties = null,
            string ipAddress = "Unknown",
            string userAgent = "Unknown",
            string? additionalInfo = null)
        {
            return new AuditLog(
                new AuditLogId(Guid.NewGuid()),
                userId,
                username,
                action,
                entityType,
                entityId,
                entityName,
                oldValues,
                newValues,
                changedProperties,
                ipAddress,
                userAgent,
                additionalInfo);
        }
    }
}
