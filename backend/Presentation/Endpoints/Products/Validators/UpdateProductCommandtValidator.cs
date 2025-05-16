using Application.Products.Commands;
using FluentValidation;

namespace Presentation.Endpoints.Products.Validators;

public class UpdateProductCommandtValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandtValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).NotEmpty().NotNull();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}
