using Application.Products.Queries;
using Application.Products.Queries.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Queries;

namespace Presentation.Endpoints.Products;

public class ProductsEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the endpoint for retrieving products.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure the routes.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v{version:apiVersion}/products", async (
                [AsParameters] GetProductsQuery query,
                [FromServices] IValidator<GetProductsQuery> validator,
                [FromServices] IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>> handler) =>
        {
            query.Normalize();

            var validationResult = await validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await handler.Handle(query, CancellationToken.None);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error);
        })
        .WithName("GetProducts")
        .WithTags("Products")
        .WithSummary("Retrieve a paginated list of products.")
        .WithDescription("Gets a paginated list of products from the catalog. Supports filtering and pagination via query parameters.")
        .Produces<PagedResult<ProductResponse>>(StatusCodes.Status200OK, "application/json")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithOpenApi(operation =>
        {
            operation.Parameters[0].Description = "Query parameters for filtering and paginating products.";
            operation.Responses["200"].Description = "A paginated list of products.";
            operation.Responses["400"].Description = "Invalid query parameters.";
            operation.Responses["500"].Description = "Internal server error.";
            return operation;
        });

        app.MapGet("/api/v{version:apiVersion}/products/{id:int}", async (
            int id,
            [FromServices] IValidator<GetProductByIdQuery> validator,
            [FromServices] IQueryHandler<GetProductByIdQuery, ProductResponse> handler) =>
        {
            var query = new GetProductByIdQuery(id);
            var validationResult = await validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await handler.Handle(query, CancellationToken.None);
            return !result.IsSuccess ? Results.NotFound(result.Error) : Results.Ok(result.Value);
        })
        .WithName("GetProductById")
        .WithTags("Products")
        .WithSummary("Retrieve details of a specific product by ID.")
        .WithDescription("Gets details of a product from the catalog by its ID.")
        .Produces<ProductResponse>(StatusCodes.Status200OK, "application/json")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}