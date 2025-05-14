using System.Reflection;
using Application.Dependencies;
using HealthChecks.UI.Client;
using Infrastructure.Dependencies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Presentation.Dependencies;
using Presentation.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

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

app.UseExceptionHandler();

await app.RunAsync();

namespace Presentation
{
    public partial class Program;
}