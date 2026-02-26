using FluentValidation;

namespace BetashipEcommerce.APP.Commands.Products.DiscontinueProduct;

public sealed class DiscontinueProductCommandValidator : AbstractValidator<DiscontinueProductCommand>
{
    public DiscontinueProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Discontinuation reason is required.")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
    }
}
