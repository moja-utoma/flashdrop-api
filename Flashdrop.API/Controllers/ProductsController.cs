using Flashdrop.API.Extensions;
using Flashdrop.Application.Products;
using Flashdrop.Application.Products.DTOs.Requests;
using Flashdrop.Application.Products.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace Flashdrop.API.Controllers;

/// <summary>
/// Manages product operations including retrieval, creation, updates, and deletion.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ProductsController
    (IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>List of all products.</returns>
    /// <response code="200">Returns the list of products.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductForListDto>>> GetProducts(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The requested product.</returns>
    /// <response code="200">Returns the product.</response>
    /// <response code="404">Product not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        return this.ToActionResult(product);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="productDto">The product data to create.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The created product resource.</returns>
    /// <response code="201">Product created successfully.</response>
    /// <response code="400">Invalid product data.</response>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest productDto, CancellationToken cancellationToken)
    {
        await _productService.CreateAsync(productDto, cancellationToken);
        return Created();
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The product ID to update.</param>
    /// <param name="productDto">The updated product data.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>No content response on successful update.</returns>
    /// <response code="204">Product updated successfully.</response>
    /// <response code="400">Invalid product data.</response>
    /// <response code="404">Product not found.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest productDto, CancellationToken cancellationToken)
    {
        await _productService.UpdateAsync(id, productDto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    /// <param name="id">The product ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>No content response on successful deletion.</returns>
    /// <response code="204">Product deleted successfully.</response>
    /// <response code="404">Product not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
