using System.Reflection;
using Application.Dependencies;
using DotNetEnv;
using HealthChecks.UI.Client;
using Infrastructure.Dependencies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Presentation.Dependencies;
using Presentation.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithMachineName()
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Host.UseSerilog();

builder.Services.AddSwaggerGen();

builder.Services
    .AddApplicationModule()
    .AddPresentationModule()
    .AddInfrastructureModule(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseGlobalExceptionHandling();

await app.RunAsync();

namespace Presentation
{
    public partial class Program;
}