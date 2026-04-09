using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagaOrchestrator.Application.Sagas.DTOs;
using SagaOrchestrator.Application.Sagas.Interfaces;

namespace SagaOrchestrator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SaleSagasController : ControllerBase
{
    private readonly ISaleSagaService _saleSagaService;

    public SaleSagasController(ISaleSagaService saleSagaService)
    {
        _saleSagaService = saleSagaService;
    }

    /// <summary>
    /// Inicia una nueva venta.
    /// </summary>
    /// <param name="request">Datos necesarios para iniciar la venta.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información de la venta creada, incluyendo su estado inicial.
    /// </returns>
    /// <response code="201">Venta creada correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="500">Ocurrió un error interno durante la creación de la venta.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SaleSagaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(SaleSagaResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateSaleSagaRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var response = await _saleSagaService.CreateAsync(request, cancellationToken);

        return response.Status is "Completed"
            ? CreatedAtAction(nameof(GetById), new { id = response.SagaId }, response)
            : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    /// <summary>
    /// Obtiene el detalle de una venta mediante su identificador.
    /// </summary>
    /// <param name="id">Identificador único de la saga de venta.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información y el estado actual de la saga de venta.
    /// </returns>
    /// <response code="200">Saga de venta encontrada correctamente.</response>
    /// <response code="404">No se encontró una saga de venta con el identificador proporcionado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SaleSagaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await _saleSagaService.GetByIdAsync(id, cancellationToken);

        if (response is null)
            return NotFound(new { message = "Saga no encontrada." });

        return Ok(response);
    }
}