using AuthService.Application.Auth.DTOs;
using AuthService.Application.Auth.Interfaces;
using AuthService.Domain.Entities;

using Microsoft.AspNetCore.Identity;

namespace AuthService.Application.Auth.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    private readonly ITokenGenerator _tokenGenerator;
    private readonly IPasswordHasher<AppUser> _passwordHasher;


    public AuthService(IUserRepository userRepository, ITokenGenerator tokenGenerator, IPasswordHasher<AppUser> passwordHasher)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();

        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);

        if (user is null)
            return null;

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return null;


        var tokenResult = _tokenGenerator.Generate(user);

        return new LoginResponse(
            tokenResult.Token,
            tokenResult.ExpiresAtUtc,
            user.Username,
            user.FullName,
            user.Role
        );
    }
}