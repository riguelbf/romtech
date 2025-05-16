using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;

namespace Application.Products.Commands.Handlers;

public class ReductionProductStockCommandHandler(IProductRepository repository) : ICommandHandler<ReductionProductStockCommand, int>
{
    public async Task<Result<int>> Handle(ReductionProductStockCommand stockCommand, CancellationToken cancellationToken)
    {
        var stockRemoved = await repository.RemoveStockAsync(stockCommand.Id!.Value, stockCommand.Quantity,cancellationToken);
            
        return !stockRemoved 
            ? Result<int>.Failure($"Product {stockCommand.Id} not found") 
            : Result<int>.Success(stockCommand.Id!.Value);
    }
}
