using BasketService.Repositories;
using MassTransit;
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
        .WriteTo.Seq(seqUrl); // SEQ Adresi
});

// 1. Redis Configuration
// Registers StackExchange.Redis implementation for IDistributedCache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// 2. Repository Registration (Dependency Injection)
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// 3. MassTransit (RabbitMQ) Configuration
// Currently setting up the transport. Consumers will be added later.
builder.Services.AddMassTransit(x => {
    x.UsingRabbitMq((context, cfg) => {
        // Konfigürasyondan "RabbitMQ:Host" değerini oku, yoksa localhost kullan
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        
        cfg.Host(rabbitHost, "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

// 4. API & Swagger Configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var builderApp = builder.Build();

// 5. HTTP Request Pipeline
if (builderApp.Environment.IsDevelopment())
{
    builderApp.UseSwagger();
    builderApp.UseSwaggerUI();
}

builderApp.UseAuthorization();
builderApp.UseSerilogRequestLogging();
builderApp.MapControllers();

Console.WriteLine(">>> BASKET SERVICE BASLATILIYOR... <<<");

builderApp.Run();