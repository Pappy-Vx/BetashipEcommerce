using BetashipEcommerce.CORE.Customers.Entities;
using BetashipEcommerce.CORE.Customers.Enums;
using BetashipEcommerce.CORE.Customers.Events;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Customers
{
    public sealed class Customer : AuditableAggregateRoot<CustomerId>
    {
        private readonly List<CustomerAddress> _addresses = new();

        public Email Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public CustomerStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

        private Customer(
            CustomerId id,
            Email email,
            string firstName,
            string lastName) : base(id)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Status = CustomerStatus.Active;
            CreatedAt = DateTime.UtcNow;
        }

        private Customer() : base() { } // For EF Core

        public static Result<Customer> Create(string email, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<Customer>(CustomerErrors.InvalidFirstName);

            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<Customer>(CustomerErrors.InvalidLastName);

            var emailResult = Email.Create(email);
            if (!emailResult.IsSuccess)
                return Result.Failure<Customer>(emailResult.Error);

            var customer = new Customer(
                new CustomerId(Guid.NewGuid()),
                emailResult.Value,
                firstName,
                lastName);

            customer.RaiseDomainEvent(new CustomerCreatedDomainEvent(customer.Id, customer.Email.Value));

            return Result.Success(customer);
        }

        public Result UpdateProfile(string firstName, string lastName, string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure(CustomerErrors.InvalidFirstName);

            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure(CustomerErrors.InvalidLastName);

            FirstName = firstName;
            LastName = lastName;

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var phoneResult = PhoneNumber.Create(phoneNumber);
                if (!phoneResult.IsSuccess)
                    return Result.Failure(phoneResult.Error);

                PhoneNumber = phoneResult.Value;
            }

            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new CustomerProfileUpdatedDomainEvent(Id));

            return Result.Success();
        }

        public Result AddAddress(CustomerAddress address)
        {
            if (_addresses.Any(a => a.IsDefault))
            {
                address.SetAsDefault(false);
            }
            else
            {
                address.SetAsDefault(true);
            }

            _addresses.Add(address);
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result SetDefaultAddress(Guid addressId)
        {
            var address = _addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return Result.Failure(CustomerErrors.AddressNotFound);

            foreach (var addr in _addresses)
            {
                addr.SetAsDefault(false);
            }

            address.SetAsDefault(true);
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Deactivate()
        {
            if (Status == CustomerStatus.Inactive)
                return Result.Failure(CustomerErrors.AlreadyInactive);

            Status = CustomerStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new CustomerDeactivatedDomainEvent(Id));

            return Result.Success();
        }

        public Result Reactivate()
        {
            if (Status == CustomerStatus.Active)
                return Result.Failure(CustomerErrors.AlreadyActive);

            Status = CustomerStatus.Active;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new CustomerReactivatedDomainEvent(Id));

            return Result.Success();
        }
    }
}
