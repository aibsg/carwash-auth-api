using carwash_auth_api;
using carwash_auth_api.Data;
using carwash_auth_api.Repositories;
using carwash_auth_api.Services;
using carwash_auth_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация JWT
builder.Services.AddControllers();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// База данных (EF Core)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => {
            npgsqlOptions.CommandTimeout(10);
            npgsqlOptions.EnableRetryOnFailure();
        }), ServiceLifetime.Scoped
    );

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<SubjectRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthServece, AuthService>();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.MapControllers();
app.Run();

