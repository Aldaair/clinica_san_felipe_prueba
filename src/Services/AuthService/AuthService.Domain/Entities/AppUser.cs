namespace AuthService.Domain.Entities;

public sealed class AppUser
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string PasswordHash { get; set; } = default!;

    public AppUser(string username, string fullName, string role, string passwordHash)
    {
        Username = username;
        FullName = fullName;
        Role = role;
        PasswordHash = passwordHash;
    }
    public void UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

}