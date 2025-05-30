using Application.Products.Queries.Dtos;
using Infrastructure.DataBase.Repositories.Products;
using SharedKernel;
using SharedKernel.Queries;

namespace Application.Products.Queries.Handlers;

public class GetProductsQueryHandler(IProductRepository repository)
    : IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>
{
    /// <summary>
    /// Handles a query for retrieving all products.
    /// </summary>>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result object containing the list of products.</returns>
    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var (products, totalCount) = await repository.GetPagedAsync(query.PageNumber!.Value, query.PageSize!.Value);
        
        var responseItems = products.Select(p => new ProductResponse {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock
        });

        var response = new PagedResult<ProductResponse>()
        {
            PageSize = query.PageSize.Value,
            PageNumber = query.PageNumber.Value,
            TotalCount = totalCount,
            Items = responseItems
        };

        return Result<PagedResult<ProductResponse>>.Success(response);
    }
}