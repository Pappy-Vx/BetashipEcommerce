namespace BetashipEcommerce.APP.Queries.Customers.GetCustomerById;

public sealed record CustomerDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Status,
    DateTime CreatedAt,
    IReadOnlyList<CustomerAddressDto> Addresses);

public sealed record CustomerAddressDto(
    Guid Id,
    string Label,
    string Street,
    string City,
    string State,
    string Country,
    string PostalCode,
    bool IsDefault);
