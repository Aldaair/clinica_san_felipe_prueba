using AuthService.Application.Auth.DTOs;

namespace AuthService.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}