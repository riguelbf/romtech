using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;

namespace Application.Products.Commands;

public class DeleteProductCommandHandler(IProductRepository repository)
{
    public async Task<Result<int>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var deleted = await repository.SoftDeleteAsync(command.Id, cancellationToken);

        return deleted
            ? Result<int>.Success(command.Id)
            : Result<int>.Failure($"Product {command.Id} not found or already deleted");
    }
}
