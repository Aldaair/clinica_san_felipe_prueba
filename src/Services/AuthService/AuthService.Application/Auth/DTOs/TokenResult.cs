namespace AuthService.Application.Auth.DTOs;

public sealed record TokenResult(
    string Token,
    DateTime ExpiresAtUtc
);