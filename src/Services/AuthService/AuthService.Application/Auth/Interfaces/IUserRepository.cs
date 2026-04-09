using AuthService.Domain.Entities;        

namespace AuthService.Application.Auth.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}       