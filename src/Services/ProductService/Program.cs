using MassTransit; // <-- BU EKLENDİ
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Configuration
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. MediatR Configuration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// 3. MassTransit & RabbitMQ Configuration
builder.Services.AddMassTransit(x =>
{
    // Default Port: 5672, User: guest, Pass: guest
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

// 4. API & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Auto-Migration
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
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// 6. Middleware
app.UseSwagger();
app.UseSwaggerUI();

// HttpsRedirection is disabled to avoid port conflicts in local microservices environment

app.UseAuthorization();

app.MapControllers();

app.Run();