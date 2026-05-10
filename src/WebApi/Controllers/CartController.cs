using Application.Dtos.RequestDto;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = "Customer,Admin")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        /// <summary>
        /// Get the logged-in customer's cart
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCart()
        {
            _logger.LogInformation("GET /cart");
            var cart = await _cartService.GetCartAsync();
            return Ok(cart);
        }

        /// <summary>
        /// Add an item to the cart
        /// </summary>
        [HttpPost("items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddItem([FromBody] CartItemRequestDto request)
        {
            _logger.LogInformation("POST /cart/items - ProductId: {ProductId}", request.ProductId);
            var cart = await _cartService.AddItemAsync(request);
            return Ok(cart);
        }

        /// <summary>
        /// Update a cart item's quantity
        /// </summary>
        [HttpPut("items/{cartItemId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem([FromRoute] Guid cartItemId, [FromBody] CartItemRequestDto request)
        {
            _logger.LogInformation("PUT /cart/items/{CartItemId}", cartItemId);
            var cart = await _cartService.UpdateItemAsync(cartItemId, request);
            return Ok(cart);
        }

        /// <summary>
        /// Remove an item from the cart
        /// </summary>
        [HttpDelete("items/{cartItemId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItem([FromRoute] Guid cartItemId)
        {
            _logger.LogInformation("DELETE /cart/items/{CartItemId}", cartItemId);
            var cart = await _cartService.RemoveItemAsync(cartItemId);
            return Ok(cart);
        }

        /// <summary>
        /// Clear all items from the cart
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ClearCart()
        {
            _logger.LogInformation("DELETE /cart");
            await _cartService.ClearCartAsync();
            return NoContent();
        }

        /// <summary>
        /// Merge guest cart into logged-in customer's cart
        /// </summary>
        [HttpPost("merge")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MergeCart([FromBody] MergeCartRequestDto request)
        {
            _logger.LogInformation("POST /cart/merge");
            var cart = await _cartService.MergeCartAsync(request);
            return Ok(cart);
        }
    }
}