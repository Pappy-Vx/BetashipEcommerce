using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Customers.Events
{
    public sealed record CustomerCreatedDomainEvent(
    CustomerId CustomerId,
    string Email) : DomainEvent;

    public sealed record CustomerProfileUpdatedDomainEvent(
        CustomerId CustomerId) : DomainEvent;

    public sealed record CustomerDeactivatedDomainEvent(
        CustomerId CustomerId) : DomainEvent;

    public sealed record CustomerReactivatedDomainEvent(
        CustomerId CustomerId) : DomainEvent;
}
