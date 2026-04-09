using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Products.DTOs;
using ProductService.Application.Products.DTOs.Saga;
using ProductService.Application.Products.Interfaces;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductAppService _productAppService;
    private readonly IProductSagaService _productSagaService;


    public ProductsController(IProductAppService productAppService, IProductSagaService productSagaService)
    {
        _productAppService = productAppService;
        _productSagaService = productSagaService;
    }

    /// <summary>
    /// Registra un nuevo producto en el sistema.
    /// </summary>
    /// <param name="request">Datos necesarios para crear el producto.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información del producto creado.
    /// </returns>
    /// <response code="201">Producto creado correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var response = await _productAppService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza la información de un producto existente.
    /// </summary>
    /// <param name="id">Identificador único del producto.</param>
    /// <param name="request">Datos actualizados del producto.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información actualizada del producto.
    /// </returns>
    /// <response code="200">Producto actualizado correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="404">No se encontró un producto con el identificador proporcionado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var response = await _productAppService.UpdateAsync(id, request, cancellationToken);

        if (response is null)
            return NotFound(new { message = "Producto no encontrado." });

        return Ok(response);
    }

    /// <summary>
    /// Obtiene la lista de todos los productos registrados.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna una colección con todos los productos registrados.
    /// </returns>
    /// <response code="200">Listado de productos obtenido correctamente.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _productAppService.GetAllAsync(cancellationToken);
        return Ok(response);
    }
    /// <summary>
    /// Obtiene el detalle de un producto mediante su identificador.
    /// </summary>
    /// <param name="id">Identificador único del producto.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna la información del producto solicitado.
    /// </returns>
    /// <response code="200">Producto encontrado correctamente.</response>
    /// <response code="404">No se encontró un producto con el identificador proporcionado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await _productAppService.GetByIdAsync(id, cancellationToken);

        if (response is null)
            return NotFound(new { message = "Producto no encontrado." });

        return Ok(response);
    }
    /// <summary>
    /// Actualiza en lote el stock de productos como parte del proceso de compra.
    /// </summary>
    /// <param name="request">Datos del lote de productos a actualizar.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna el resultado de la actualización en lote de productos.
    /// </returns>
    /// <response code="200">Actualización en lote realizada correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("purchase-batch-update")]
    [ProducesResponseType(typeof(ProductPurchaseBatchUpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PurchaseBatchUpdate(
    [FromBody] ProductPurchaseBatchUpdateRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _productSagaService.ApplyPurchaseBatchUpdateAsync(request, cancellationToken);
        return Ok(response);
    }
    /// <summary>
    /// Revierte una actualización en lote de productos realizada durante el proceso de compra.
    /// </summary>
    /// <param name="request">Datos necesarios para revertir la actualización en lote.</param>
    /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
    /// <returns>
    /// Retorna el resultado del rollback de la actualización en lote.
    /// </returns>
    /// <response code="200">Rollback de actualización en lote realizado correctamente.</response>
    /// <response code="400">La solicitud es inválida o el modelo enviado no cumple las validaciones.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("purchase-batch-update/rollback")]
    [ProducesResponseType(typeof(ProductPurchaseBatchUpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PurchaseBatchUpdateRollback(
        [FromBody] ProductPurchaseBatchRollbackRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _productSagaService.RollbackPurchaseBatchUpdateAsync(request, cancellationToken);
        return Ok(response);
    }
}