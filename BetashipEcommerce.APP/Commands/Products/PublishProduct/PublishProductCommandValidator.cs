using FluentValidation;

namespace BetashipEcommerce.APP.Commands.Products.PublishProduct;

public sealed class PublishProductCommandValidator : AbstractValidator<PublishProductCommand>
{
    public PublishProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
