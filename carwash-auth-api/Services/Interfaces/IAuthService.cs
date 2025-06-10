using carwash_auth_api.DTOs;
using LoginRequest = carwash_auth_api.DTOs.LoginRequest;
using RegisterRequest = carwash_auth_api.DTOs.RegisterRequest;

namespace carwash_auth_api.Services.Interfaces;

public interface IAuthServece
{
    public Task<AuthResponse> LoginAsync (LoginRequest request); 
    public Task<AuthResponse> RegisterAsync (RegisterRequest request);
    public Task<AuthResponse> RefreshAsync  (string refreshToken);
    
    
}