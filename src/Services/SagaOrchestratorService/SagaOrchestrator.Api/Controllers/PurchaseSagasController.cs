using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagaOrchestrator.Application.Sagas.DTOs;
using SagaOrchestrator.Application.Sagas.Interfaces;

namespace SagaOrchestrator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PurchaseSagasController : ControllerBase
{
    private readonly IPurchaseSagaService _purchaseSagaService;

    public PurchaseSagasController(IPurchaseSagaService purchaseSagaService)
    {
        _purchaseSagaService = purchaseSagaService;
    }
    /// <summary>
    /// Inicia una nueva compra para coordinar el flujo distribuido del proceso de compra.
    /// </summary>
    /// <param name="request">Datos necesarios para iniciar la compra.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información de la saga creada, incluyendo su estado inicial.
    /// </returns>
    /// <response code="201">compra creada correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="500">Ocurrió un error interno durante la compra.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseSagaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(PurchaseSagaResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseSagaRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var response = await _purchaseSagaService.CreateAsync(request, cancellationToken);

        return response.Status is "Completed"
            ? CreatedAtAction(nameof(GetById), new { id = response.SagaId }, response)
            : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    /// <summary>
    /// Obtiene el detalle de compra mediante su identificador.
    /// </summary>
    /// <param name="id">Identificador único de la compra.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información y el estado actual de la compra.
    /// </returns>
    /// <response code="200">Compra encontrada correctamente.</response>
    /// <response code="404">No se encontró una compra con el identificador proporcionado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PurchaseSagaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await _purchaseSagaService.GetByIdAsync(id, cancellationToken);

        if (response is null)
            return NotFound(new { message = "Saga no encontrada." });

        return Ok(response);
    }
}