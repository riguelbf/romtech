
namespace Presentation.Dependencies;

public static class PresentationModule
{
    public static IServiceCollection AddPresentationModule(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();

        return services;
    }
}