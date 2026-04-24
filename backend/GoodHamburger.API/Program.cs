using GoodHamburger.API.Hubs;
using GoodHamburger.API.Middlewares;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// === Services ===
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

// === CORS (para React frontend) ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// === Dependency Injection ===

// Repositories (Singleton — in-memory, dados estáticos)
builder.Services.AddSingleton<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddSingleton<IPromotionRepository, PromotionRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

// Services
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// === Middleware Pipeline ===
app.UseMiddleware<DomainErrorMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.MapControllers();
app.MapHub<OrderHub>("/hubs/orders");

app.Run();
