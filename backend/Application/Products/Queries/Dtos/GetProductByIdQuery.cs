namespace Application.Products.Queries.Dtos;

public record GetProductByIdQuery(int Id) : IQuery<ProductResponse>;
