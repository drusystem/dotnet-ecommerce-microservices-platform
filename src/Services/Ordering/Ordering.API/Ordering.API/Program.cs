using BuildingBlocks.Common.Middleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Validators;
using Ordering.Domain.Repositories;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/ordering-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando Ordering Service");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/ordering-.txt", rollingInterval: RollingInterval.Day));

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Database
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    // builder.Services.AddDbContext<OrderingDbContext>(options =>
    //     options.UseSqlServer(connectionString));
    builder.Services.AddDbContext<OrderingDbContext>(options =>
    options.UseSqlServer(connectionString,
        sql => sql.MigrationsAssembly("Ordering.Infrastructure")));
        


    // Repositories
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();

    // MediatR
    builder.Services.AddMediatR(cfg => {
        cfg.RegisterServicesFromAssembly(typeof(Ordering.Application.Commands.CreateOrderCommand).Assembly);
    });

    // FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();
    builder.Services.AddFluentValidationAutoValidation()
        .AddFluentValidationClientsideAdapters();

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddSqlServer(connectionString!, name: "OrderingDB");

    // CORS
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


    // crea la BD y aplica migraciones
    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            for (int i = 1; i <= 10; i++)
            {
                try
                {
                    logger.LogInformation("Intentando aplicar migraciones... intento {Attempt}", i);
                    db.Database.Migrate();
                    logger.LogInformation("Migraciones aplicadas correctamente");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "SQL Server aún no está listo, reintentando...");
                    Thread.Sleep(5000);
                }
            }
        }

    }

    // Configure the HTTP request pipeline.
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();

    app.UseCors("AllowAll");
    app.UseAuthorization();

    // Health Check endpoints
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapControllers();

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