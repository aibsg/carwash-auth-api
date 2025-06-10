using carwash_auth_api.Data;
using carwash_auth_api.DTOs;
using carwash_auth_api.Models;
using carwash_auth_api.Repositories;
using carwash_auth_api.Services.Interfaces;
using RegisterRequest = carwash_auth_api.DTOs.RegisterRequest;

namespace carwash_auth_api.Services;

public class AuthService : IAuthServece
{
    private readonly UserRepository _userRepository;
    private readonly SubjectRepository _subjectRepository;
    private readonly IJwtService _jwtService;

    public AuthService(UserRepository userRepository, IJwtService jwtService, SubjectRepository subjectRepository) 
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _subjectRepository = subjectRepository;
    }
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmail(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Неверный email или пароль");
        
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        
        await _userRepository.Update(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }
    

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.GetByEmail(request.Email) != null)
            throw new Exception("Email уже существует");
        
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
        
        await _subjectRepository.Add(subject);
        await _userRepository.Add(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshToken(refreshToken);

        if (user == null)
            throw new Exception("Пользователь с таким токеном не найден, залогиньтесь снова");
        
        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
         
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        
        await _userRepository.Update(user);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }
}