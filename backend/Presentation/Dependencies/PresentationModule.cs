using FluentValidation;
using Presentation.Endpoints.Products;
using Presentation.Endpoints.Products.Validators;

namespace Presentation.Dependencies;

public static class PresentationModule
{
    public static IServiceCollection AddPresentationModule(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddProblemDetails()
            .AddValidators();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<GetProductsQueryValidator>();
        services.AddValidatorsFromAssemblyContaining<GetProductByIdQueryValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateProductCommandtValidator>();
        services.AddValidatorsFromAssemblyContaining<AddStockCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<ReductionProductStockCommandValidator>();

        return services;
    }
}