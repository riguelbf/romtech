using Application.Products.Commands;
using FluentValidation;

namespace Presentation.Endpoints.Products.Validators;

public class AddStockCommandValidator : AbstractValidator<AddStockCommand>
{
    public AddStockCommandValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}
