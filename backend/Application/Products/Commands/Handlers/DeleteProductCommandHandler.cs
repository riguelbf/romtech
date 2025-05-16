using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;

namespace Application.Products.Commands.Handlers;

public class DeleteProductCommandHandler(IProductRepository repository)
{
    /// <summary>
    /// Handles a command for deleting a product by its ID.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result object containing the ID of the deleted product, or a failure message if not found.</returns>
    public async Task<Result<int>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var deleted = await repository.SoftDeleteAsync(command.Id, cancellationToken);

        return deleted
            ? Result<int>.Success(command.Id)
            : Result<int>.Failure($"Product {command.Id} not found or already deleted");
    }
}
