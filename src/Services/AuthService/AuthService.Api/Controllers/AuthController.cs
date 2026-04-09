using System.Security.Claims;
using AuthService.Application.Auth.DTOs;
using AuthService.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Autentica a un usuario y devuelve un token JWT si las credenciales son válidas.
    /// </summary>
    /// <param name="request">Credenciales de acceso del usuario.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna <see cref="LoginResponse"/> con el token de autenticación y los datos básicos del usuario.
    /// </returns>
    /// <response code="200">Inicio de sesión exitoso.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="401">Credenciales inválidas.</response>
    /// <response code="500">Error interno del servidor.</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var response = await _authService.LoginAsync(request, cancellationToken);

        if (response is null)
        {
            return Unauthorized(new
            {
                message = "Usuario o contraseña inválidos."
            });
        }

        return Ok(response);
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado actual.
    /// </summary>
    /// <returns>
    /// Retorna los datos del usuario autenticado a partir del token enviado en la solicitud.
    /// </returns>
    /// <response code="200">Información del usuario obtenida correctamente.</response>
    /// <response code="401">El usuario no está autenticado o el token no es válido.</response>
    /// <response code="500">Error interno del servidor.</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        return Ok(new
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            fullName = User.FindFirstValue(ClaimTypes.Name),
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}