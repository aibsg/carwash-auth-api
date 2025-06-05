using carwash_auth_api;
using carwash_auth_api.Data;
using carwash_auth_api.Services;
using carwash_auth_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация JWT
builder.Services.AddControllers();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// База данных (EF Core)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")/*,
        npgsqlOptions => 
        {
            npgsqlOptions.CommandTimeout(60); // 60 секунд вместо 30 по умолчанию
            npgsqlOptions.EnableRetryOnFailure(3); // Повторные попытки
        }*/
    )
);

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.MapControllers();
app.Run();

