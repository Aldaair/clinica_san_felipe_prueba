using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseService.Application.Purchases.DTOs;
using PurchaseService.Application.Purchases.Interfaces;

namespace PurchaseService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PurchasesController : ControllerBase
{
    private readonly IPurchaseAppService _purchaseAppService;

    public PurchasesController(IPurchaseAppService purchaseAppService)
    {
        _purchaseAppService = purchaseAppService;
    }

    /// <summary>
    /// Registra una nueva compra en el sistema.
    /// </summary>
    /// <param name="request">Datos necesarios para registrar la compra.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la compra registrada correctamente.
    /// </returns>
    /// <response code="201">Compra registrada correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var response = await _purchaseAppService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = response.IdCompraCab }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cancela una compra registrada previamente.
    /// </summary>
    /// <param name="id">Identificador único de la compra.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la compra actualizada con el estado de cancelación.
    /// </returns>
    /// <response code="200">Compra cancelada correctamente.</response>
    /// <response code="404">No se encontró una compra con el identificador proporcionado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var response = await _purchaseAppService.CancelAsync(id, cancellationToken);

        if (response is null)
            return NotFound(new { message = "Compra no encontrada." });

        return Ok(response);
    }

    /// <summary>
    /// Obtiene la lista de todas las compras registradas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna una colección con todas las compras registradas.
    /// </returns>
    /// <response code="200">Listado de compras obtenido correctamente.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PurchaseResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _purchaseAppService.GetAllAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Obtiene el detalle de una compra por su identificador.
    /// </summary>
    /// <param name="id">Identificador único de la compra.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información de la compra solicitada.
    /// </returns>
    /// <response code="200">Compra encontrada correctamente.</response>
    /// <response code="404">No se encontró una compra con el identificador proporcionado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await _purchaseAppService.GetByIdAsync(id, cancellationToken);

        if (response is null)
            return NotFound(new { message = "Compra no encontrada." });

        return Ok(response);
    }
}