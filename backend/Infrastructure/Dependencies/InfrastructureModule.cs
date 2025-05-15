using Infrastructure.DataBase.DataContext;
using Infrastructure.DataBase.Repositories.Base;
using Infrastructure.DataBase.Repositories.Products;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Dependencies;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddDatabase()
            .AddRepositories(configuration)
            .AddHealthChecks(configuration);
    

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "public"))
                .UseSnakeCaseNamingConvention()
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine, LogLevel.Information));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        
        return services;
    }

    
    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks();

        return services;
    }
}