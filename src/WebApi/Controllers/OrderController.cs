using Application.Dtos.RequestDto;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Checkout - create order and get Paystack payment link 
        /// </summary>
        [HttpPost("checkout")]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            _logger.LogInformation("POST /order/checkout");
            var result = await _orderService.CheckoutAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Get logged-in customer's orders 
        /// </summary>
        [HttpGet("my-orders")]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET /order/my-orders");
            var orders = await _orderService.GetMyOrdersAsync(page, pageSize);
            return Ok(orders);
        }

        /// <summary>
        /// Get order by ID (Customer can only see their own, Admin can see all)
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            _logger.LogInformation("GET /order/{OrderId}", id);
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
                return NotFound(new { message = $"Order with ID '{id}' was not found." });

            return Ok(order);
        }

        /// <summary>
        /// Get order by order number 
        /// </summary>
        [HttpGet("number/{orderNumber}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByOrderNumber([FromRoute] string orderNumber)
        {
            _logger.LogInformation("GET /order/number/{OrderNumber}", orderNumber);
            var order = await _orderService.GetByOrderNumberAsync(orderNumber);

            if (order == null)
                return NotFound(new { message = $"Order '{orderNumber}' was not found." });

            return Ok(order);
        }

        /// <summary>
        /// Get all orders (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET /order - Page: {Page}", page);
            var orders = await _orderService.GetAllOrdersAsync(page, pageSize);
            return Ok(orders);
        }

        /// <summary>
        /// Update order status (Admin only)
        /// </summary>
        [HttpPut("{id:guid}/status")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateOrderStatusDto request)
        {
            _logger.LogInformation("PUT /order/{OrderId}/status - Status: {Status}", id, request.Status);

            var order = await _orderService.UpdateStatusAsync(id, request.Status);

            if (order == null)
                return NotFound(new { message = $"Order with ID '{id}' was not found." });

            return Ok(order);
        }

        /// <summary>
        /// Paystack webhook - called by Paystack after payment (Public - no auth)
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Webhook()
        {
            _logger.LogInformation("POST /order/webhook - Paystack webhook received");

            // Read raw body - important for signature verification
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            var signature = Request.Headers["x-paystack-signature"].ToString();

            await _orderService.HandleWebhookAsync(rawBody, signature);

            // Always return 200 to Paystack even if we skip processing
            return Ok();
        }
    }
}