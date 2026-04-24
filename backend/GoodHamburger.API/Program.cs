using System.Text;
using GoodHamburger.API.Middlewares;
using GoodHamburger.Application.Hubs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// === Services ===
builder.Services.AddControllers();
builder.Services.AddSignalR();

// === JWT Authentication ===
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"] ?? "SecretKeyMuitoLongaESegura1234567890");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Em prod configurar Issuer e Audience
        ValidateAudience = false
    };
});

// === Swagger / OpenAPI ===
builder.Services.AddOpenApi();

// === CORS ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000") // Origens do React
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Importante para SignalR
    });
});

// === Dependency Injection ===

// Repositories (Singleton — in-memory)
builder.Services.AddSingleton<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddSingleton<IPromotionRepository, PromotionRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// === Middleware Pipeline ===
app.UseMiddleware<DomainErrorMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderHub>("/hubs/orders");

app.Run();
