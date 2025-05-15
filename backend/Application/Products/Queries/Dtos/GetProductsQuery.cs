using SharedKernel.Queries;

namespace Application.Products.Queries.Dtos;

public record GetProductsQuery : PaginationQuery, IQuery<IEnumerable<PagedResult<ProductResponse>>>, IQuery<IEnumerable<ProductResponse>>, IQuery<PagedResult<ProductResponse>>
{
   // Add properties if needed (e.g., filters)
}