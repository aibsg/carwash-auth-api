namespace carwash_auth_api.DTOs;

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}