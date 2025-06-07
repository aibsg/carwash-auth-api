using System.Diagnostics;
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
        var currentUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        if (currentUser != null)
            return BadRequest("Email is already taken");
        
        var subject = new Subject
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password) 
        };
        
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        
        await _db.Subjects.AddAsync(subject);
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });

    }

    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        
        var email = request.Email;
        var sw = Stopwatch.StartNew();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        sw.Stop();
        Console.WriteLine($"[DB] Login: поиск по Email занял {sw.ElapsedMilliseconds} мс");
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized();
        
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        var sw2 = Stopwatch.StartNew();
        await _db.SaveChangesAsync();
        sw2.Stop();
        Console.WriteLine($"[DB] Login: Созранение занял {sw2.ElapsedMilliseconds} мс");

        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] string refreshToken)
    {
        var users = await _db.Users.ToListAsync();
        var user = await _db.Users.FirstOrDefaultAsync(u => 
            u.RefreshToken == refreshToken);

        if (user == null)
            return Unauthorized();
        
        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
         
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
    
    [HttpPost("validate")]
    public IActionResult ValidateToken([FromBody] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();
        
        var principal = _jwtService.ValidateToken(token);

        if (principal == null)
            return Unauthorized();

        var userId = principal.FindFirst("userId")?.Value;
        return Ok(new { userId });
    }
}