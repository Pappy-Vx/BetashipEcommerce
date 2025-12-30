using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Customers.ValueObjects
{
    public sealed partial class Email : ValueObject
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        private Email() { } // For EF Core

        public static Result<Email> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure<Email>(CustomerErrors.InvalidEmail);

            email = email.Trim().ToLowerInvariant();

            if (!EmailRegex().IsMatch(email))
                return Result.Failure<Email>(CustomerErrors.InvalidEmail);

            return Result.Success(new Email(email));
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
        private static partial Regex EmailRegex();
    }
}
