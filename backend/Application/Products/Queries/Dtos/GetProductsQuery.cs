namespace Application.Products.Queries.Dtos;

public record GetProductsQuery : IQuery<IEnumerable<ProductResponse>>
{
    // Add properties if needed (e.g., filters)
}