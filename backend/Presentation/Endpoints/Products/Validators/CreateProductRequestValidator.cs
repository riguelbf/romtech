using Application.Products.Commands;
using FluentValidation;

namespace Presentation.Endpoints.Products.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}
