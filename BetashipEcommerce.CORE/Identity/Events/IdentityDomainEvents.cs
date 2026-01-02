using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Identity.Enums;
using BetashipEcommerce.CORE.Identity.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Identity.Events
{
    public sealed record UserCreatedDomainEvent(
    UserId UserId,
    string Username,
    string Email) : DomainEvent;

    public sealed record UserLinkedToCustomerDomainEvent(
        UserId UserId,
        CustomerId CustomerId) : DomainEvent;

    public sealed record UserRoleAddedDomainEvent(
        UserId UserId,
        UserRole Role) : DomainEvent;

    public sealed record UserRoleRemovedDomainEvent(
        UserId UserId,
        UserRole Role) : DomainEvent;

    public sealed record UserLoggedInDomainEvent(
        UserId UserId,
        string Username,
        string IpAddress) : DomainEvent;

    public sealed record UserLockedOutDomainEvent(
        UserId UserId,
        string Username,
        int FailedAttempts) : DomainEvent;

    public sealed record UserUnlockedDomainEvent(
        UserId UserId,
        string Username) : DomainEvent;

    public sealed record UserPasswordChangedDomainEvent(
        UserId UserId) : DomainEvent;

    public sealed record UserEmailVerifiedDomainEvent(
        UserId UserId,
        string Email) : DomainEvent;

    public sealed record UserDeactivatedDomainEvent(
        UserId UserId,
        string Username,
        string Reason) : DomainEvent;

    public sealed record UserReactivatedDomainEvent(
        UserId UserId,
        string Username) : DomainEvent;
}
