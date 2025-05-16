using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;
using Domain.Products;

namespace Application.Products.Commands;

public class CreateProductHandler : ICommandHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _repository;

    public CreateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the creation of a new product with the given <paramref name="command"/>.
    /// </summary>
    /// <param name="command">The command containing the product's details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result object containing the ID of the new product.</returns>
    public async Task<Result<int>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            Stock = command.Stock
        };

        await _repository.AddAsync(product);
        return Result<int>.Success(product.Id);
    }
}
