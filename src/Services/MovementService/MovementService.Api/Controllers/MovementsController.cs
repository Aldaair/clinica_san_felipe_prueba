using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovementService.Application.Movements.DTOs;
using MovementService.Application.Movements.Interfaces;

namespace MovementService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class MovementsController : ControllerBase
{
    private readonly IMovementAppService _movementAppService;

    public MovementsController(IMovementAppService movementAppService)
    {
        _movementAppService = movementAppService;
    }
    /// <summary>
    /// Registra un movimiento de entrada de stock en el inventario.
    /// </summary>
    /// <param name="request">Datos necesarios para registrar la entrada de stock.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información del movimiento registrado.
    /// </returns>
    /// <response code="200">Movimiento de entrada registrado correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("entry")]
    [ProducesResponseType(typeof(MovementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterEntry([FromBody] MovementEntryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var response = await _movementAppService.RegisterEntryAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = ex.Message,
                detail = ex.InnerException?.Message
            });
        }
    }
    /// <summary>
    /// Revierte un movimiento de entrada de stock previamente registrado.
    /// </summary>
    /// <param name="request">Datos necesarios para revertir la entrada de stock.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información del movimiento revertido.
    /// </returns>
    /// <response code="200">Movimiento de entrada revertido correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="404">No se encontró el movimiento de entrada a revertir.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("entry/reverse")]
    [ProducesResponseType(typeof(MovementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReverseEntry([FromBody] MovementReverseEntryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var response = await _movementAppService.ReverseEntryAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = ex.Message,
                detail = ex.InnerException?.Message
            });
        }
    }
    /// <summary>
    /// Registra un movimiento de salida de stock en el inventario.
    /// </summary>
    /// <param name="request">Datos necesarios para registrar la salida de stock.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información del movimiento registrado.
    /// </returns>
    /// <response code="200">Movimiento de salida registrado correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("exit")]
    [ProducesResponseType(typeof(MovementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterExit([FromBody] MovementExitRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var response = await _movementAppService.RegisterExitAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = ex.Message,
                detail = ex.InnerException?.Message
            });
        }
    }
    /// <summary>
    /// Obtiene el stock actual de todos los productos registrados.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna una colección con el stock actual de todos los productos.
    /// </returns>
    /// <response code="200">Stock actual obtenido correctamente.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("stock")]
    [ProducesResponseType(typeof(IReadOnlyList<StockResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentStock(CancellationToken cancellationToken)
    {
        var response = await _movementAppService.GetCurrentStockAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Obtiene el stock actual de un producto específico.
    /// </summary>
    /// <param name="productId">Identificador único del producto.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna el stock actual del producto solicitado.
    /// </returns>
    /// <response code="200">Stock del producto obtenido correctamente.</response>
    /// <response code="404">No se encontró el producto solicitado o no tiene stock registrado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("stock/{productId:int}")]
    [ProducesResponseType(typeof(StockResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStockByProductId(int productId, CancellationToken cancellationToken)
    {
        var response = await _movementAppService.GetStockByProductIdAsync(productId, cancellationToken);
        return Ok(response);
    }
    /// <summary>
    /// Obtiene el historial de movimientos de un producto específico.
    /// </summary>
    /// <param name="productId">Identificador único del producto.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la lista de movimientos asociados al producto.
    /// </returns>
    /// <response code="200">Historial de movimientos obtenido correctamente.</response>
    /// <response code="404">No se encontró el producto solicitado o no tiene historial registrado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("history/{productId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<MovementHistoryItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHistoryByProductId(int productId, CancellationToken cancellationToken)
    {
        var response = await _movementAppService.GetHistoryByProductIdAsync(productId, cancellationToken);
        return Ok(response);
    }
    /// <summary>
    /// Revierte un movimiento de salida de stock previamente registrado.
    /// </summary>
    /// <param name="request">Datos necesarios para revertir la salida de stock.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información del movimiento revertido.
    /// </returns>
    /// <response code="200">Movimiento de salida revertido correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="404">No se encontró el movimiento de salida a revertir.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("exit/reverse")]
    [ProducesResponseType(typeof(MovementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReverseExit([FromBody] MovementReverseExitRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var response = await _movementAppService.ReverseExitAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = ex.Message,
                detail = ex.InnerException?.Message
            });
        }
    }
}