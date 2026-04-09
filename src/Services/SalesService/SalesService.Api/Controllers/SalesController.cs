using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesService.Application.Sales.DTOs;
using SalesService.Application.Sales.Interfaces;

namespace SalesService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SalesController : ControllerBase
{
    private readonly ISaleAppService _saleAppService;

    public SalesController(ISaleAppService saleAppService)
    {
        _saleAppService = saleAppService;
    }

    /// <summary>
    /// Registra una nueva venta en el sistema.
    /// </summary>
    /// <param name="request">Datos necesarios para registrar la venta.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información de la venta creada.
    /// </returns>
    /// <response code="201">Venta registrada correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="401">No autorizado. El token es inválido o no fue enviado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var response = await _saleAppService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = response.IdVentaCab }, response);
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
    /// Cancela una venta registrada previamente.
    /// </summary>
    /// <param name="id">Identificador único de la venta.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información de la venta actualizada con el estado de cancelación.
    /// </returns>
    /// <response code="200">Venta cancelada correctamente.</response>
    /// <response code="401">No autorizado. El token es inválido o no fue enviado.</response>
    /// <response code="404">No se encontró una venta con el identificador proporcionado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var response = await _saleAppService.CancelAsync(id, cancellationToken);

        if (response is null)
            return NotFound(new { message = "Venta no encontrada." });

        return Ok(response);
    }

    /// <summary>
    /// Obtiene la lista de todas las ventas registradas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna una colección con todas las ventas registradas.
    /// </returns>
    /// <response code="200">Listado de ventas obtenido correctamente.</response>
    /// <response code="401">No autorizado. El token es inválido o no fue enviado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _saleAppService.GetAllAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Obtiene el detalle de una venta mediante su identificador.
    /// </summary>
    /// <param name="id">Identificador único de la venta.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información de la venta solicitada.
    /// </returns>
    /// <response code="200">Venta encontrada correctamente.</response>
    /// <response code="401">No autorizado. El token es inválido o no fue enviado.</response>
    /// <response code="404">No se encontró una venta con el identificador proporcionado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await _saleAppService.GetByIdAsync(id, cancellationToken);

        if (response is null)
            return NotFound(new { message = "Venta no encontrada." });

        return Ok(response);
    }
}