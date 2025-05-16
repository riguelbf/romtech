using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;

namespace Application.Products.Commands.Handlers;

public class UpdateProductCommandHandler(IProductRepository repository) : ICommandHandler<UpdateProductCommand, int>
{
    /// <summary>
    /// Handles a command for updating a product.
    /// </summary>
    /// <param name="command">The command containing the product's details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result object containing the ID of the updated product, or a failure message if not found.</returns>
    public async Task<Result<int>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(command.Id);
        if (product is null)
        {
            return Result<int>.Failure($"Product {command.Id} not found");
        }

        product.Name = command.Name;
        product.Description = command.Description;
        product.Price = command.Price;
        product.Stock = command.Stock;

        await repository.UpdateAsync(product, cancellationToken);
        return Result<int>.Success(product.Id);    
    }
}
