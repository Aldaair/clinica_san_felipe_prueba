using AuthService.Application.Auth.DTOs;
using AuthService.Domain.Entities;

namespace AuthService.Application.Auth.Interfaces;

public interface ITokenGenerator
{
    TokenResult Generate(AppUser user);
}