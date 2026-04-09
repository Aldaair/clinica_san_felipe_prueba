using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Auth.DTOs;

/// <summary>
/// Solicitud de autenticación.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Nombre de usuario.
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}