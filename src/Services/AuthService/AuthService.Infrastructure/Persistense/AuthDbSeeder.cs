using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence;

public static class AuthDbSeeder
{
    public static async Task SeedAsync(AuthDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Users.AnyAsync())
            return;

        var hasher = new PasswordHasher<AppUser>();

        var admin = new AppUser(
            username: "admin",
            fullName: "Administrador San Felipe",
            role: "Admin",
            passwordHash: string.Empty
        );

        admin.UpdatePasswordHash(hasher.HashPassword(admin, "Admin123*"));

        await dbContext.Users.AddAsync(admin);
        await dbContext.SaveChangesAsync();
    }
}