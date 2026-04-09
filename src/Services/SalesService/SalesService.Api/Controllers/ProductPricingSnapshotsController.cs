using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesService.Application.Sales.DTOs;
using SalesService.Application.Sales.Interfaces;

namespace SalesService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProductPricingSnapshotsController : ControllerBase
{
    private readonly IProductPricingSnapshotAppService _appService;

    public ProductPricingSnapshotsController(IProductPricingSnapshotAppService appService)
    {
        _appService = appService;
    }

/// <summary>
    /// Crea o actualiza el snapshot de precios de productos utilizado por el servicio de ventas.
    /// </summary>
    /// <param name="request">Datos del snapshot de precios a registrar o actualizar.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna un mensaje de confirmación cuando el snapshot es actualizado correctamente.
    /// </returns>
    /// <response code="200">Snapshot actualizado correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="401">No autorizado. El token es inválido o no fue enviado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("upsert")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upsert(
        [FromBody] ProductPricingSnapshotUpsertRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        await _appService.UpsertAsync(request, cancellationToken);
        return Ok(new { message = "Snapshot actualizado correctamente." });
    }
}