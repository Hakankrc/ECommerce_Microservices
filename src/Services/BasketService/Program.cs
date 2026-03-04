using BasketService.Repositories;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddMassTransit(x =>
{
    // Using RabbitMQ with MassTransit
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
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
builderApp.MapControllers();

builderApp.Run();