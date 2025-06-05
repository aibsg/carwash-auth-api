using carwash_auth_api.DTOs;
using carwash_auth_api.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = carwash_auth_api.DTOs.LoginRequest;
using RegisterRequest = Microsoft.AspNetCore.Identity.Data.RegisterRequest;

namespace carwash_auth_api.Services.Interfaces;

public interface IAuthServece
{
    public Task<AuthResponse> Login (LoginRequest request); 
    public Task<IActionResult> Register (RegisterRequest request);
    public Task<AuthResponse> Refresh  (RefreshTokenRequest request);
    
}