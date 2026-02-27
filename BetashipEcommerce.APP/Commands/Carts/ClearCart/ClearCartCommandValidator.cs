using FluentValidation;

namespace BetashipEcommerce.APP.Commands.Carts.ClearCart;

public sealed class ClearCartCommandValidator : AbstractValidator<ClearCartCommand>
{
    public ClearCartCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}
