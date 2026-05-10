using Application.Dtos.RequestDto;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get all products with optional search, filtering, and pagination (Public)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            _logger.LogInformation(
                "GET /products - SearchTerm: {SearchTerm}, CategoryId: {CategoryId}, Page: {Page}",
                searchTerm, categoryId, page);

            var result = await _productService.SearchAsync(searchTerm, categoryId, page, pageSize, sortBy);
            return Ok(result);
        }

        /// <summary>
        /// Get a product by ID (Public)
        /// </summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            _logger.LogInformation("GET /products/{ProductId}", id);

            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound(new { message = $"Product with ID '{id}' was not found." });

            return Ok(product);
        }

        /// <summary>
        /// Create a new product (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto request)
        {
            _logger.LogInformation("POST /products - Creating product: {Name}", request.Name);
            var product = await _productService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        /// <summary>
        /// Update a product (Admin only)
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] ProductUpdateDto request)
        {
            _logger.LogInformation("PUT /products/{ProductId}", id);

            var product = await _productService.UpdateAsync(id, request);

            if (product == null)
                return NotFound(new { message = $"Product with ID '{id}' was not found." });

            return Ok(product);
        }

        /// <summary>
        /// Delete a product (Admin only)
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            _logger.LogInformation("DELETE /products/{ProductId}", id);

            var result = await _productService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = $"Product with ID '{id}' was not found." });

            return NoContent();
        }
    }
}