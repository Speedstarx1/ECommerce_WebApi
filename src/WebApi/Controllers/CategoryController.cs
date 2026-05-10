using Application.Dtos.RequestDto;
using Application.Services.Interfaces;
using Asp.Versioning;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{

    [Authorize]
    [Produces("application/json")]

    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }
        /// <summary>
        /// Create a new category (Admin Only)
        /// </summary>
        /// <param name="request">Category details</param>
        /// <returns>Created category details</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CategoryRequestDto request)
        {
            _logger.LogInformation("POST /categories - Creating category: {Name}", request.Name);
            var category = await _categoryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /categories");
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get a category by ID (Public)
        /// </summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            _logger.LogInformation("GET /categories/{CategoryId}", id);
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound(new { message = $"Category with ID '{id}' was not found." });

            return Ok(category);
        }

        /// <summary>
        /// Update an existing category (Admin only)
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CategoryRequestDto request)
        {
            _logger.LogInformation("PUT /categories/{CategoryId}", id);
            var category = await _categoryService.UpdateAsync(id, request);

            if (category == null)
                return NotFound(new { message = $"Category with ID '{id}' was not found." });

            return Ok(category);
        }

        /// <summary>
        /// Delete a category (Admin only)
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            _logger.LogInformation("DELETE /categories/{CategoryId}", id);
            var result = await _categoryService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = $"Category with ID '{id}' was not found." });

            return NoContent();
        }
    }
}