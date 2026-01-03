using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Identity.Entities;
using BetashipEcommerce.CORE.Identity.Enums;
using BetashipEcommerce.CORE.Identity.Events;
using BetashipEcommerce.CORE.Identity.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using PermissionsClass = BetashipEcommerce.CORE.Identity.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Identity
{
    /// <summary>
    /// User aggregate for authentication and authorization
    /// Separate from Customer (a user might not be a customer, e.g., admin)
    /// </summary>
    public sealed class User : AuditableAggregateRoot<UserId>
    {
        private readonly List<UserRole> _roles = new();
        private readonly List<string> _permissions = new();
        private readonly List<UserSession> _sessions = new();

        public string Username { get; private set; }
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName => $"{FirstName} {LastName}";

        // Optional link to Customer aggregate (if user is also a customer)
        public CustomerId? CustomerId { get; private set; }

        public UserStatus Status { get; private set; }
        public bool IsEmailVerified { get; private set; }
        public DateTime? EmailVerifiedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public DateTime? LastPasswordChangedAt { get; private set; }
        public int FailedLoginAttempts { get; private set; }
        public DateTime? LockedOutUntil { get; private set; }

        public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();
        public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();
        public IReadOnlyCollection<UserSession> Sessions => _sessions.AsReadOnly();

        private User(
            UserId id,
            string username,
            Email email,
            string passwordHash,
            string firstName,
            string lastName) : base(id)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            Status = UserStatus.Active;
            IsEmailVerified = false;
            CreatedAt = DateTime.UtcNow;
            FailedLoginAttempts = 0;
        }

        private User() : base()
        {
            Username = string.Empty;
            Email = null!;
            PasswordHash = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        public static Result<User> Create(
            string username,
            string email,
            string passwordHash,
            string firstName,
            string lastName)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Result.Failure<User>(IdentityErrors.InvalidUsername);

            if (username.Length < 3 || username.Length > 50)
                return Result.Failure<User>(IdentityErrors.UsernameLengthInvalid);

            var emailResult = Email.Create(email);
            if (!emailResult.IsSuccess)
                return Result.Failure<User>(emailResult.Error);

            if (string.IsNullOrWhiteSpace(passwordHash))
                return Result.Failure<User>(IdentityErrors.InvalidPasswordHash);

            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<User>(IdentityErrors.InvalidFirstName);

            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<User>(IdentityErrors.InvalidLastName);

            var user = new User(
                new UserId(Guid.NewGuid()),
                username.ToLowerInvariant(),
                emailResult.Value,
                passwordHash,
                firstName,
                lastName);

            user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, user.Username, user.Email.Value));

            return Result.Success(user);
        }

        /// <summary>
        /// Link user to customer account
        /// </summary>
        public Result LinkToCustomer(CustomerId customerId)
        {
            if (CustomerId != null)
                return Result.Failure(IdentityErrors.UserAlreadyLinkedToCustomer);

            CustomerId = customerId;
            RaiseDomainEvent(new UserLinkedToCustomerDomainEvent(Id, customerId));

            return Result.Success();
        }

        /// <summary>
        /// Add role to user
        /// </summary>
        public Result AddRole(UserRole role)
        {
            if (_roles.Contains(role))
                return Result.Failure(IdentityErrors.RoleAlreadyAssigned);

            _roles.Add(role);

            // Add role permissions
            _permissions.AddRange(GetRolePermissions(role));

            RaiseDomainEvent(new UserRoleAddedDomainEvent(Id, role));

            return Result.Success();
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        public Result RemoveRole(UserRole role)
        {
            if (!_roles.Contains(role))
                return Result.Failure(IdentityErrors.RoleNotFound);

            _roles.Remove(role);

            // Remove role permissions (careful with overlapping permissions)
            var rolePermissions = GetRolePermissions(role);
            foreach (var permission in rolePermissions)
            {
                if (!HasPermissionFromOtherRoles(permission, role))
                {
                    _permissions.Remove(permission);
                }
            }

            RaiseDomainEvent(new UserRoleRemovedDomainEvent(Id, role));

            return Result.Success();
        }

        /// <summary>
        /// Check if user has specific permission
        /// </summary>
        public bool HasPermission(string permission)
        {
            return Status == UserStatus.Active
                && !IsLockedOut()
                && _permissions.Contains(permission);
        }

        /// <summary>
        /// Check if user has specific role
        /// </summary>
        public bool HasRole(UserRole role)
        {
            return _roles.Contains(role);
        }

        /// <summary>
        /// Record successful login
        /// </summary>
        public Result RecordLogin(string ipAddress, string userAgent)
        {
            if (Status != UserStatus.Active)
                return Result.Failure(IdentityErrors.UserNotActive);

            if (IsLockedOut())
                return Result.Failure(IdentityErrors.UserLockedOut);

            LastLoginAt = DateTime.UtcNow;
            FailedLoginAttempts = 0;

            var session = UserSession.Create(ipAddress, userAgent);
            _sessions.Add(session);

            RaiseDomainEvent(new UserLoggedInDomainEvent(Id, Username, ipAddress));

            return Result.Success();
        }

        /// <summary>
        /// Record failed login attempt
        /// </summary>
        public Result RecordFailedLogin()
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= 5)
            {
                LockOut(TimeSpan.FromMinutes(30));
                RaiseDomainEvent(new UserLockedOutDomainEvent(Id, Username, FailedLoginAttempts));
            }

            return Result.Success();
        }

        /// <summary>
        /// Lock out user
        /// </summary>
        public void LockOut(TimeSpan duration)
        {
            LockedOutUntil = DateTime.UtcNow.Add(duration);
        }

        /// <summary>
        /// Check if user is locked out
        /// </summary>
        public bool IsLockedOut()
        {
            return LockedOutUntil.HasValue && DateTime.UtcNow < LockedOutUntil.Value;
        }

        /// <summary>
        /// Unlock user
        /// </summary>
        public Result Unlock()
        {
            if (!IsLockedOut())
                return Result.Failure(IdentityErrors.UserNotLockedOut);

            LockedOutUntil = null;
            FailedLoginAttempts = 0;

            RaiseDomainEvent(new UserUnlockedDomainEvent(Id, Username));

            return Result.Success();
        }

        /// <summary>
        /// Change password
        /// </summary>
        public Result ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                return Result.Failure(IdentityErrors.InvalidPasswordHash);

            PasswordHash = newPasswordHash;
            LastPasswordChangedAt = DateTime.UtcNow;

            RaiseDomainEvent(new UserPasswordChangedDomainEvent(Id));

            return Result.Success();
        }

        /// <summary>
        /// Verify email
        /// </summary>
        public Result VerifyEmail()
        {
            if (IsEmailVerified)
                return Result.Failure(IdentityErrors.EmailAlreadyVerified);

            IsEmailVerified = true;
            EmailVerifiedAt = DateTime.UtcNow;

            RaiseDomainEvent(new UserEmailVerifiedDomainEvent(Id, Email.Value));

            return Result.Success();
        }

        /// <summary>
        /// Deactivate user
        /// </summary>
        public Result Deactivate(string reason)
        {
            if (Status == UserStatus.Inactive)
                return Result.Failure(IdentityErrors.UserAlreadyInactive);

            Status = UserStatus.Inactive;

            RaiseDomainEvent(new UserDeactivatedDomainEvent(Id, Username, reason));

            return Result.Success();
        }

        /// <summary>
        /// Reactivate user
        /// </summary>
        public Result Reactivate()
        {
            if (Status == UserStatus.Active)
                return Result.Failure(IdentityErrors.UserAlreadyActive);

            Status = UserStatus.Active;
            FailedLoginAttempts = 0;
            LockedOutUntil = null;

            RaiseDomainEvent(new UserReactivatedDomainEvent(Id, Username));

            return Result.Success();
        }

        private bool HasPermissionFromOtherRoles(string permission, UserRole excludeRole)
        {
            return _roles
                .Where(r => r != excludeRole)
                .SelectMany(GetRolePermissions)
                .Contains(permission);
        }

        private static List<string> GetRolePermissions(UserRole role)
        {
            return role switch
            {

                UserRole.SuperAdmin => PermissionsClass.GetAllPermissions(),
                UserRole.Admin => PermissionsClass.AdminPermissions,
                UserRole.InventoryManager => PermissionsClass.InventoryPermissions,
                UserRole.OrderManager => PermissionsClass.OrderPermissions,
                UserRole.CustomerSupport => PermissionsClass.SupportPermissions,
                UserRole.Customer => PermissionsClass.CustomerPermissions,
                _ => new List<string>()
            };
        }
    }
}
