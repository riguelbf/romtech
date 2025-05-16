using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;

namespace Application.Products.Commands;

public class UpdateProductCommandHandler(IProductRepository repository) : ICommandHandler<UpdateProductCommand, int>
{
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
