using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/gateway-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando API Gateway");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/gateway-.txt", rollingInterval: RollingInterval.Day));

    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    // Health Checks - Usar nombres de contenedores en Docker
    var catalogUrl = builder.Environment.EnvironmentName == "Docker" 
        ? "http://catalog-api:80/health" 
        : "http://localhost:5001/health";
    
    var basketUrl = builder.Environment.EnvironmentName == "Docker"
        ? "http://basket-api:80/health"
        : "http://localhost:5002/health";

    var orderingUrl = builder.Environment.EnvironmentName == "Docker"
        ? "http://ordering-api:80/health"
        : "http://localhost:5003/health";

    builder.Services.AddHealthChecks()
        .AddUrlGroup(new Uri(catalogUrl), name: "Catalog Service")
        .AddUrlGroup(new Uri(basketUrl), name: "Basket Service")
        .AddUrlGroup(new Uri(orderingUrl), name: "Ordering Service");

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            policyBuilder =>
            {
                policyBuilder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseCors("AllowAll");

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapReverseProxy();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar");
}
finally
{
    Log.CloseAndFlush();
}