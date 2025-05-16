using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;

namespace Application.Products.Commands.Handlers;

public class AddStockCommandHandler(IProductRepository repository) : ICommandHandler<AddStockCommand, int>
{
  
    /// <summary>
    /// Handles the adding of stock to a product.
    /// </summary>
    /// <param name="command">The command containing the product's ID and the quantity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public async Task<Result<int>> Handle(AddStockCommand command, CancellationToken cancellationToken)
    {
        if (command.Quantity <= 0)
            return Result<int>.Failure("Quantity must be greater than zero.");

        var product = await repository.GetByIdAsync(command.Id!, cancellationToken);
        
        if (product == null || product.IsDeleted)
            return Result<int>.Failure("Product not found");

        var result =await repository.AddStockAsync(product.Id, command.Quantity, cancellationToken);

        return result ? Result<int>.Success(product.Id) : Result<int>.Failure("Failed to add stock");
    }
}
