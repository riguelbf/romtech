using Application.Products.Queries.Dtos;
using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;

namespace Application.Products.Queries;

public class GetProductByIdQueryHandler(IProductRepository repository)
    : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(query.Id);
        if (product == null)
            return Result<ProductResponse>.Failure($"Product with ID {query.Id} not found.");

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock
        };
        return Result<ProductResponse>.Success(response);
    }
}
