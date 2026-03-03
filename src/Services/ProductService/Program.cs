using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Configuration
// Registers the DbContext with SQL Server connection string
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. MediatR Configuration
// Automatically scans the assembly to register all Commands, Queries, and Handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// 3. API & Swagger Configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Auto-Migration Strategy
// Automatically applies pending migrations when the service starts
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
        // Log errors if migration fails (Best Practice)
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// 5. Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI();

// Note: HttpsRedirection is disabled for local development to prevent port issues with Docker/Gateway
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();