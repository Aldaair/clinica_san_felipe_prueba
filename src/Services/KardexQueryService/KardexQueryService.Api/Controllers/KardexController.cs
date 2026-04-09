using KardexQueryService.Application.Kardex.DTOs;
using KardexQueryService.Application.Kardex.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KardexQueryService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class KardexController : ControllerBase
{
    private readonly IKardexQueryService _kardexQueryService;

    public KardexController(IKardexQueryService kardexQueryService)
    {
        _kardexQueryService = kardexQueryService;
    }

    /// <summary>
    /// Obtiene el kardex general de productos.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la lista del kardex con la información consolidada de inventario por producto.
    /// </returns>
    /// <response code="200">Kardex obtenido correctamente.</response>
    /// <response code="401">No autorizado. El token es inválido o no fue enviado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<KardexResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetKardex(CancellationToken cancellationToken)
    {
        var response = await _kardexQueryService.GetKardexAsync(cancellationToken);
        return Ok(response);
    }

/// <summary>
    /// Obtiene el detalle de movimientos de un producto específico en el kardex.
    /// </summary>
    /// <param name="productId">Identificador único del producto.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la lista de movimientos asociados al producto solicitado.
    /// </returns>
    /// <response code="200">Movimientos del producto obtenidos correctamente.</response>
    /// <response code="401">No autorizado. El token es inválido o no fue enviado.</response>
    /// <response code="404">No se encontró información de movimientos para el producto indicado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("{productId:int}/movements")]
    [ProducesResponseType(typeof(IReadOnlyList<KardexMovementResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMovementsByProductId(int productId, CancellationToken cancellationToken)
    {
        var response = await _kardexQueryService.GetProductMovementsAsync(productId, cancellationToken);
        return Ok(response);
    }
}