using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetashipEcommerce.CORE.SharedKernel;

namespace BetashipEcommerce.CORE.Identity
{
    public static class IdentityErrors
    {
        public static readonly Error InvalidUsername = new("Identity.InvalidUsername",
            "Username cannot be empty");

        public static readonly Error UsernameLengthInvalid = new("Identity.UsernameLengthInvalid",
            "Username must be between 3 and 50 characters");

        public static readonly Error InvalidPasswordHash = new("Identity.InvalidPasswordHash",
            "Invalid password hash");

        public static readonly Error InvalidFirstName = new("Identity.InvalidFirstName",
            "First name cannot be empty");

        public static readonly Error InvalidLastName = new("Identity.InvalidLastName",
            "Last name cannot be empty");

        public static readonly Error UserAlreadyLinkedToCustomer = new("Identity.UserAlreadyLinkedToCustomer",
            "User is already linked to a customer account");

        public static readonly Error RoleAlreadyAssigned = new("Identity.RoleAlreadyAssigned",
            "Role is already assigned to user");

        public static readonly Error RoleNotFound = new("Identity.RoleNotFound",
            "Role not found for user");

        public static readonly Error UserNotActive = new("Identity.UserNotActive",
            "User account is not active");

        public static readonly Error UserLockedOut = new("Identity.UserLockedOut",
            "User account is locked out");

        public static readonly Error UserNotLockedOut = new("Identity.UserNotLockedOut",
            "User is not locked out");

        public static readonly Error EmailAlreadyVerified = new("Identity.EmailAlreadyVerified",
            "Email is already verified");

        public static readonly Error UserAlreadyInactive = new("Identity.UserAlreadyInactive",
            "User is already inactive");

        public static readonly Error UserAlreadyActive = new("Identity.UserAlreadyActive",
            "User is already active");

        public static readonly Error NotFound = new("Identity.NotFound",
            "User not found");

        public static readonly Error InvalidCredentials = new("Identity.InvalidCredentials",
            "Invalid username or password");

        public static readonly Error InsufficientPermissions = new("Identity.InsufficientPermissions",
            "Insufficient permissions for this operation");
    }

}
