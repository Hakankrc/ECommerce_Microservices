using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Register YARP (Reverse Proxy)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Rate Limiting Configuration
// Limits requests to prevent abuse (5 requests per 10 seconds)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed-window", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

var app = builder.Build();


app.UseRateLimiter(); 

app.MapReverseProxy(); 

app.Run();