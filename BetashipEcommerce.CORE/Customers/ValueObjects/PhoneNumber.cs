using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Customers.ValueObjects
{
    public sealed partial class PhoneNumber : ValueObject
    {
        public string Value { get; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        private PhoneNumber() { } // For EF Core

        public static Result<PhoneNumber> Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return Result.Failure<PhoneNumber>(CustomerErrors.InvalidPhoneNumber);

            phoneNumber = phoneNumber.Trim();

            // Remove common formatting characters
            phoneNumber = phoneNumber.Replace("-", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(" ", "");

            if (!PhoneRegex().IsMatch(phoneNumber))
                return Result.Failure<PhoneNumber>(CustomerErrors.InvalidPhoneNumber);

            return Result.Success(new PhoneNumber(phoneNumber));
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        [GeneratedRegex(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled)]
        private static partial Regex PhoneRegex();
    }
}
