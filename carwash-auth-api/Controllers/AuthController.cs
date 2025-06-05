using carwash_auth_api.Data;
using carwash_auth_api.DTOs;
using carwash_auth_api.Models;
using carwash_auth_api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginRequest = carwash_auth_api.DTOs.LoginRequest;
using RegisterRequest = carwash_auth_api.DTOs.RegisterRequest;

namespace carwash_auth_api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly AppDbContext _db;

    public AuthController(JwtService jwtService, AppDbContext db)
    {
        _jwtService = jwtService;
        _db = db;
    }

    [HttpGet]
    [Route("test")]
    public async Task<IActionResult> Test()
    {
        return Ok(await _db.Users.FirstOrDefaultAsync(u => u.Email == "string"));
    }




    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email) != null)
        {
            return BadRequest("Email is already taken");
        }

        if (await _db.Subjects.FirstOrDefaultAsync(s => s.FirstName == request.FirstName) != null)
        {
            return BadRequest("User with such data already exists");
        }

        var subject = new Subject()
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            SubjectId = subject.Id,
            SubjectInfo = subject,
            IsPartner = request.IsPartner,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password) // Авто-соль + хеш
        };

        _db.Subjects.Add(subject);
        _db.Users.Add(user);

        // 4. Генерация токенов
        var accessToken = _jwtService.GenerateAccessToken(user.Id);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // 5. Сохранение refresh токена
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _db.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });

    }


    // Логин
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var email = request.Email;
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized();

        // Генерируем токены
        var accessToken = _jwtService.GenerateAccessToken(user.Id);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Сохраняем Refresh Token в БД
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _db.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });
    }

    // Обновление токена
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] string refreshToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => 
            u.RefreshToken == refreshToken && 
            u.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null)
            return Unauthorized();

        // Новые токены
        var newAccessToken = _jwtService.GenerateAccessToken(user.Id);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Обновляем Refresh Token в БД
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _db.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });
    }
}