namespace AuthService.Application.Auth.DTOs;

/// <summary>
/// Respuesta de autenticación exitosa.
/// </summary>
public sealed record LoginResponse(
    /// <summary>
    /// Token JWT generado para el usuario autenticado.
    /// </summary>
    string AccessToken,
    /// <summary>
    /// Fecha y hora de expiración del token en UTC.
    /// </summary>
    DateTime ExpiresAtUtc,
    /// <summary>
    /// Nombre de usuario.
    /// </summary>
    string Username,
    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    string FullName,
    /// <summary>
    /// Rol asignado al usuario.
    /// </summary>
    string Role
);