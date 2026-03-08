using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Consumers;
using OrderService.Infrastructure;
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

// SQL Server Configuration
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MassTransit & RabbitMQ Configuration
builder.Services.AddMassTransit(x => {
    x.UsingRabbitMq((context, cfg) => {
        // Read "RabbitMQ:Host" from configuration, if not found use localhost
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        
        cfg.Host(rabbitHost, "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

// Add Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-Migration on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<OrderDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.MapControllers();

Console.WriteLine(">>> ORDER SERVICE BASLATILIYOR... <<<");

app.Run();