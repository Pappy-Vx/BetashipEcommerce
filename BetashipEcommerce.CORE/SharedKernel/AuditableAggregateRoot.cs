using BetashipEcommerce.CORE.SharedKernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.SharedKernel
{
    /// <summary>
    /// Base class for aggregate roots that require audit tracking
    /// </summary>
    public abstract class AuditableAggregateRoot<TId> : AuditableEntity<TId>
        where TId : notnull
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        protected AuditableAggregateRoot(TId id) : base(id) { }
        protected AuditableAggregateRoot() : base() { }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }

}
