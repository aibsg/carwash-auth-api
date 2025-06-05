namespace carwash_auth_api.DTOs;

public class RegisterRequest
{
    public bool IsPartner { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    
    
}