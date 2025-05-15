using Application.Products.Queries.Dtos;
using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;
using SharedKernel.Queries;

namespace Application.Products.Queries;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public GetProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles a query for retrieving all products.
    /// </summary>>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result object containing the list of products.</returns>
    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _repository.GetPagedAsync(query.PageNumber, query.PageSize);
        
        var responseItems = products.Select(p => new ProductResponse {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock
        });

        var response = new PagedResult<ProductResponse>()
        {
            PageSize = query.PageSize,
            PageNumber = query.PageNumber,
            TotalCount = totalCount,
            Items = responseItems
        };

        return Result<PagedResult<ProductResponse>>.Success(response);
    }
}