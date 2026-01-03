using BetashipEcommerce.CORE.Identity.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.SharedKernel
{
    /// <summary>
    /// Base class for entities that require audit tracking
    /// </summary>
    public abstract class AuditableEntity<TId> : AggregateRoot<TId>
        where TId : notnull
    {
        public DateTime CreatedAt { get; protected set; }
        public UserId? CreatedBy { get; protected set; }
        public string? CreatedByUsername { get; protected set; }

        public DateTime? UpdatedAt { get; protected set; }
        public UserId? UpdatedBy { get; protected set; }
        public string? UpdatedByUsername { get; protected set; }

        public bool IsDeleted { get; protected set; }
        public DateTime? DeletedAt { get; protected set; }
        public UserId? DeletedBy { get; protected set; }
        public string? DeletedByUsername { get; protected set; }

        protected AuditableEntity(TId id) : base(id)
        {
            CreatedAt = DateTime.UtcNow;
        }

        protected AuditableEntity() : base() { }

        public void SetCreatedBy(UserId userId, string username)
        {
            CreatedBy = userId;
            CreatedByUsername = username;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetUpdatedBy(UserId userId, string username)
        {
            UpdatedBy = userId;
            UpdatedByUsername = username;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDeletedBy(UserId userId, string username)
        {
            DeletedBy = userId;
            DeletedByUsername = username;
            DeletedAt = DateTime.UtcNow;
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
            DeletedByUsername = null;
        }
    }
}
