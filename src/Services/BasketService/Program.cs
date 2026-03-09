using BasketService.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens; 
using Serilog;
using System.Text; 

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

// 1. Redis configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisSettings:ConnectionString"];
});

// 2. Repository Registration
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// 3. MassTransit (RabbitMQ) Configuration
builder.Services.AddMassTransit(x => {
    x.UsingRabbitMq((context, cfg) => {
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        cfg.Host(rabbitHost, "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});



var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"]
    ?? throw new Exception("JWT secret not configured for BasketService");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false, 
            ValidateAudience = false, 
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 5. API & Swagger Configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var builderApp = builder.Build();

// 6. HTTP Request Pipeline
if (builderApp.Environment.IsDevelopment())
{
    builderApp.UseSwagger();
    builderApp.UseSwaggerUI();
}

builderApp.UseSerilogRequestLogging();


builderApp.UseAuthentication(); 
builderApp.UseAuthorization();  

builderApp.MapControllers();

Console.WriteLine(">>> BASKET SERVICE BASLATILIYOR... <<<");

builderApp.Run();