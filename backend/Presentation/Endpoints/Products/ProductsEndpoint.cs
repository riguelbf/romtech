using Application.Products.Commands;
using Application.Products.Commands.Handlers;
using Application.Products.Queries;
using Application.Products.Queries.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Queries;

namespace Presentation.Endpoints.Products;

public class ProductsEndpoint : EndpointBase
{
    /// <summary>
    /// Maps the endpoint for retrieving products.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure the routes.</param>
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v{version:apiVersion}/products", async (
                [AsParameters] GetProductsQuery query,
                [FromServices] IValidator<GetProductsQuery> validator,
                [FromServices] IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>> handler) =>
        {
            query.Normalize();

            var (requestIsValid, validationResult) = await ValidateRequestAsync(query, validator);
            if (!requestIsValid) return validationResult;

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
            
            var (requestIsValid, validationResult) = await ValidateRequestAsync(query, validator);
            if (!requestIsValid)return validationResult;
            
            var result = await handler.Handle(query, CancellationToken.None);
            
            if (!result.IsSuccess || result.Value is null || result.Value.Id <= 0)
                return Results.NotFound(result.Error ?? "Product not found");
            
            return Results.Ok(result.Value);
        })
        .WithName("GetProductById")
        .WithTags("Products")
        .WithSummary("Retrieve details of a specific product by ID.")
        .WithDescription("Gets details of a product from the catalog by its ID.")
        .Produces<ProductResponse>(StatusCodes.Status200OK, "application/json")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        app.MapPost("/api/v{version:apiVersion}/products", async (
            [FromBody] CreateProductCommand command,
            [FromServices] IValidator<CreateProductCommand> validator,
            [FromServices] CreateProductHandler handler,
            CancellationToken cancellationToken) =>
        {
            var (requestIsValid, validationResult) = await ValidateRequestAsync(command, validator);
            if (!requestIsValid)return validationResult;
           
            var result = await handler.Handle(command, cancellationToken);
            
            return !result.IsSuccess ? Results.Problem(result.Error) : Results.Created($"/api/v1/products/{result.Value}", new { id = result.Value });
        })
        .WithName("CreateProduct")
        .WithTags("Products")
        .WithSummary("Create a new product.")
        .WithDescription("Adds a new product to the system.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);

        app.MapPut("/api/v{version:apiVersion}/products/{id:int}", async (
            int id,
            [FromBody] UpdateProductCommand command,
            [FromServices] IValidator<UpdateProductCommand> validator,
            [FromServices] UpdateProductCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var (requestIsValid, validationResult) = await ValidateRequestAsync(command, validator);
            if (!requestIsValid)return validationResult;
            
            if(id != command.Id) return Results.BadRequest("Id mismatch");

            var result = await handler.Handle(command, cancellationToken);
            
            if (result.IsSuccess) return Results.NoContent();
            
            return result.Error == "Product not found" ? Results.NotFound(result.Error) : Results.BadRequest(result.Error);
        })
        .WithName("UpdateProduct")
        .WithTags("Products")
        .WithSummary("Update an existing product by ID.")
        .WithDescription("Updates an existing product in the catalog by its ID.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete($"/api/v{{version:apiVersion}}/products/{{id:int}}", async (
            int id,
            [FromServices] DeleteProductCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new DeleteProductCommand(id), cancellationToken);
            return result.IsSuccess ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .WithTags("Products")
        .WithSummary("Soft delete a product by ID.")
        .WithDescription("Soft-deletes a product by setting its IsDeleted flag to true. Returns 204 if successful, 404 if not found or already deleted.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        app.MapPost($"/api/v{{version:apiVersion}}/products/{{id:int}}/stock", async (
                int id,
                [FromServices] IValidator<AddStockCommand> validator,
                [FromBody] AddStockCommand command,
                [FromServices] AddStockCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                command.Id = id;
                
                var (requestIsValid, validationResult) = await ValidateRequestAsync(command, validator);
                if (!requestIsValid) return validationResult;

                var result = await handler.Handle(command, cancellationToken);

                if (result.IsSuccess) return Results.NoContent();

                return result.Error == "Product not found" ? Results.NotFound() : Results.BadRequest(result.Error);
            })
            .WithName("AddProductStock")
            .WithTags("Products")
            .WithSummary("Add stock to an existing product.")
            .WithDescription("Increments the stock of a product by the specified quantity.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status412PreconditionFailed);

        app.MapDelete($"/api/v{{version:apiVersion}}/products/{{id:int}}/stock", async (
                int id,
                [FromServices] IValidator<ReductionProductStockCommand> validator,
                [FromBody] ReductionProductStockCommand command,
                [FromServices] ReductionProductStockCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                command.Id = id;
                
                var (requestIsValid, validationResult) = await ValidateRequestAsync(command, validator);
                if (!requestIsValid) return validationResult;
                
                var result = await handler.Handle(command, cancellationToken);
                
                if (result.IsSuccess)
                    return Results.NoContent();
                
                if (result.Error == $"Product {id} not found")
                    return Results.NotFound($"Product {id} not found");
                
                return result.Error == "Concurrency conflict" 
                    ? Results.StatusCode(StatusCodes.Status412PreconditionFailed) 
                    : Results.Problem(result.Error);
            })
            .WithName("ReductionProductStock")
            .WithTags("Products")
            .WithSummary("Reduction of product stock")
            .WithDescription("reduces the amount of stock of a given product if the stock does not become negative")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status412PreconditionFailed)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}