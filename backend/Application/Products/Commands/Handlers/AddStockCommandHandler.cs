using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;

namespace Application.Products.Commands.Handlers;

public class AddStockCommandHandler(IProductRepository repository) : ICommandHandler<AddStockCommand, int>
{
    /// <summary>
    /// Handles a command for adding stock to a product.
    /// </summary>
    /// <param name="command">The command containing the product's ID and quantity.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result object containing the ID of the product, or a failure message if it was not found.</returns>
    public async Task<Result<int>> Handle(AddStockCommand command, CancellationToken cancellationToken)
    {
        if (command.Quantity <= 0)
            return Result<int>.Failure("Quantity must be greater than zero.");

        var product = await repository.GetByIdAsync(command.Id);
        
        if (product == null)
            return Result<int>.Failure("Product not found");

        await repository.AddStockAsync(product.Id, command.Quantity, cancellationToken);

        return Result<int>.Success(product.Id);
    }
}
