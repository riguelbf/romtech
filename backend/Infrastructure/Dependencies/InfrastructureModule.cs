using Infrastructure.DataBase.DataContext;
using Infrastructure.DataBase.Repositories.Base;
using Infrastructure.DataBase.Repositories.Products;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Dependencies;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddDatabase()
            .AddRepositories()
            .ApplyMigrations()
            .AddHealthsChecks();
    

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

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        
        return services;
    }

    private static IServiceCollection AddHealthsChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks();

        return services;
    }

    private static IServiceCollection ApplyMigrations(this IServiceCollection services)
    {
        var runMigrations = Environment.GetEnvironmentVariable("RUN_MIGRATIONS");

        if (!string.Equals(runMigrations, "true", StringComparison.OrdinalIgnoreCase)) return services;
        
        using var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        return services;
    }
}