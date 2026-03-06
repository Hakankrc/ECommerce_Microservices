using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 1. JWT Ayarlarını Okuma
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];
var key = Encoding.UTF8.GetBytes(secretKey!);

// 2. Authentication (Kimlik Doğrulama) Servisini Ekle
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Dev ortamı için
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

// 3. Authorization (Yetkilendirme) Politikası Tanımla
builder.Services.AddAuthorization(options =>
{
    // "AuthenticatedUser" adında bir kural tanımlıyoruz: Sadece giriş yapmış olanlar!
    options.AddPolicy("AuthenticatedUser", policy => 
        policy.RequireAuthenticatedUser());
});

// YARP Servisi
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Rate Limiter
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

// 4. Middleware Sıralaması (Çok Önemli!)
app.UseRateLimiter(); // Önce hız sınırı

app.UseAuthentication(); // Sonra "Kimsin?" kontrolü
app.UseAuthorization();  // Sonra "Yetkin var mı?" kontrolü

app.MapReverseProxy();

app.Run();