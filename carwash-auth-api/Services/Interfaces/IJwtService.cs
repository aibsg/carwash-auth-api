using carwash_auth_api.Models;

namespace carwash_auth_api.Services.Interfaces;

public interface IJwtService
{
    public string GenerateAccessToken(User user);
    public string GenerateRefreshToken();
}