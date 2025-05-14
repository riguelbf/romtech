using Microsoft.Extensions.DependencyInjection;

namespace Application.Dependencies;

public static class ApplicationModule
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services) => services; 
}