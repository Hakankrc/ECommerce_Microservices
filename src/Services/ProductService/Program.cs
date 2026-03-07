using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure;
using Shared.Behaviors;
using System.Reflection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var seqUrl = builder.Configuration["Serilog:SeqUrl"] ?? "http://localhost:5341";

builder.Host.UseSerilog((context, configuration) => 
{
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(seqUrl);
});

// 1. Database Configuration
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Global Exception Handling Registration
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 3. MediatR & Validation Pipeline Configuration
// Registers validators and sets up the validation behavior in the pipeline
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>)); 
});

// 4. MassTransit (RabbitMQ) Configuration
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

// 5. API Controller Configuration
builder.Services.AddControllers();

// IMPORTANT: Disable default ASP.NET Core model validation filter.
// We want our FluentValidation + MediatR pipeline to handle validations manually.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// 6. Automatic Database Migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ProductDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database migration failed.");
    }
}

// 7. Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI();

// Register ExceptionHandler middleware first to catch all errors
app.UseExceptionHandler(); 
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.MapControllers();

Console.WriteLine(">>> PRODUCT SERVICE (CQRS) BASLATILIYOR... <<<");
app.Run();