using Application.Products.Commands;
using FluentValidation;

namespace Presentation.Endpoints.Products.Validators;

public class ReductionProductStockCommandValidator: AbstractValidator<ReductionProductStockCommand>
{
    public ReductionProductStockCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .NotEmpty()
            .NotNull()
            .WithMessage("Id must be greater than 0");
        
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .NotEmpty()
            .WithMessage("Quantity must be greater than 0");;
    }
}