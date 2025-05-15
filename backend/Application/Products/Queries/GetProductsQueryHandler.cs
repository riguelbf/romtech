using Application.Products.Queries.Dtos;
using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;

namespace Application.Products.Queries;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, IEnumerable<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public GetProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles a query for retrieving all products.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result object containing the list of products.</returns>
    public async Task<Result<IEnumerable<ProductResponse>>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync();
        var dtos = products.Select(p => new ProductResponse { Id = p.Id, Name = p.Name });
        return Result<IEnumerable<ProductResponse>>.Success(dtos);
    }
}