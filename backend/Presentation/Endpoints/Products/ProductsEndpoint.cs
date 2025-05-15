using Application.Products.Queries;
using Application.Products.Queries.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Endpoints.Products;

public class ProductsEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the endpoint for retrieving products.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure the routes.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async (
            [AsParameters] GetProductsQuery query,
            [FromServices] IQueryHandler<GetProductsQuery, IEnumerable<ProductResponse>> handler) =>
        {
            var result = await handler.Handle(query, CancellationToken.None);
            return result.IsSuccess ? Results. Ok(result.Value) : Results.Problem(result.Error);
        });
    }
}