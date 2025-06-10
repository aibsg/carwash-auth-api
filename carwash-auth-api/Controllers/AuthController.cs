using System.Diagnostics;
using carwash_auth_api.Data;
using carwash_auth_api.DTOs;
using carwash_auth_api.Models;
using carwash_auth_api.Services;
using carwash_auth_api.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginRequest = carwash_auth_api.DTOs.LoginRequest;
using RegisterRequest = carwash_auth_api.DTOs.RegisterRequest;

namespace carwash_auth_api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IJwtService _jwtService, AppDbContext _dbContext, IAuthServece _authServece) : ControllerBase
{
    [HttpGet]
    [Route("test")]
    public async Task<IActionResult> Test()
    {
        return Ok(await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "string"));
    }


    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var serviseResponse = await _authServece.RegisterAsync(request);
        return Ok(serviseResponse);
    }
    

    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {  
        var serviseResponse = _authServece.LoginAsync(request);
        return Ok(serviseResponse);
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] string refreshToken)
    {
        var response =  await _authServece.RefreshAsync(refreshToken);
        return Ok(response);
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
        return Ok("Айди пользователя: "+ userId );
    }
}