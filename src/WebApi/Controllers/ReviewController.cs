using Application.Dtos.RequestDto;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        /// <summary>
        /// Get all reviews for a product (Public)
        /// </summary>
        [HttpGet("product/{productId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByProduct([FromRoute] Guid productId)
        {
            _logger.LogInformation("GET /reviews/product/{ProductId}", productId);
            var reviews = await _reviewService.GetByProductIdAsync(productId);
            return Ok(reviews);
        }

        /// <summary>
        /// Create a review for a purchased product
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] ReviewRequestDto request)
        {
            _logger.LogInformation("POST /reviews - ProductId: {ProductId}", request.ProductId);
            var review = await _reviewService.CreateAsync(request);
            return CreatedAtAction(nameof(GetByProduct), new { productId = review.ProductId }, review);
        }

        /// <summary>
        /// Update your own review
        /// </summary>
        [HttpPut("{reviewId:guid}")]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid reviewId, [FromBody] ReviewRequestDto request)
        {
            _logger.LogInformation("PUT /reviews/{ReviewId}", reviewId);
            var review = await _reviewService.UpdateAsync(reviewId, request);

            if (review == null)
                return NotFound(new { message = $"Review with ID '{reviewId}' was not found." });

            return Ok(review);
        }

        /// <summary>
        /// Delete a review (Customer can delete own, Admin can delete any)
        /// </summary>
        [HttpDelete("{reviewId:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid reviewId)
        {
            _logger.LogInformation("DELETE /reviews/{ReviewId}", reviewId);
            var result = await _reviewService.DeleteAsync(reviewId);

            if (!result)
                return NotFound(new { message = $"Review with ID '{reviewId}' was not found." });

            return NoContent();
        }
    }
}