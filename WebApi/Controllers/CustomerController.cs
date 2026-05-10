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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Get all customers with optional search, filtering, and pagination
        /// </summary>
        /// <param name="searchTerm">Search by name, email, or ref number</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
        /// <param name="sortBy">Sort field (firstname, lastname, refnumber, createddate) with optional _desc suffix</param>
        /// <returns>Paginated list of customers</returns>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            _logger.LogInformation(
                "GET /customers - SearchTerm: {SearchTerm}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}",
                searchTerm, page, pageSize, sortBy);

            var result = await _customerService.SearchAsync(searchTerm, page, pageSize, sortBy);
            return Ok(result);
        }

        /// <summary>
        /// Get a customer by ID
        /// </summary>
        /// <param name="id">Customer's unique identifier</param>
        /// <returns>Customer's details or 404 if not found</returns>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            _logger.LogInformation("GET /customers/{CustomerId}", id);

            var customer = await _customerService.GetByIdAsync(id);

            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found", id);
                return NotFound(new { message = $"Customer with ID '{id}' was not found." });
            }

            return Ok(customer);
        }

        /// <summary>
        /// Get a customer by matriculation number
        /// </summary>
        /// <param name="matricNo">Customer's reference number</param>
        /// <returns>Customer's details or 404 if not found</returns>
        [HttpGet("reference_number/{refNo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByRefNumber([FromRoute] string refNo)
        {
            _logger.LogInformation("GET /customer/reference/{RefNumber}", refNo);

            var customer = await _customerService.GetAsync(refNo);

            if (customer == null)
            {
                _logger.LogWarning("Customer with Reference Number {RefNumber} not found", refNo);
                return NotFound(new { message = $"Customer with Reference number '{refNo}' was not found." });
            }

            return Ok(customer);
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="request">Customer creation request</param>
        /// <returns>Created Customer with 201 status code</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDto request)
        {
            _logger.LogInformation("POST /customers - Creating customer with email: {Email}", request.Email);

            var customer = await _customerService.CreateAsync(request);

            _logger.LogInformation("Customer created with ID: {CustomerId}", customer.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id, version = "1.0" },
                customer);
        }

        /// <summary>
        /// Update an existing customer
        /// </summary>
        /// <param name="id">Customer's unique identifier</param>
        /// <param name="request">Update request with fields to modify</param>
        /// <returns>204 No Content on success, 404 if not found</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CustomerUpdateRequest request)
        {
            _logger.LogInformation("PUT /customers/{CustomerId}", id);

            var customer = await _customerService.UpdateAsync(id, request);

            if (customer == null)
            {
                _logger.LogWarning("Cannot update - Customer with ID {CustomerId} not found", id);
                return NotFound(new { message = $"Customer with ID '{id}' was not found." });
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a customer (soft delete)
        /// </summary>
        /// <param name="id">Customer's unique identifier</param>
        /// <returns>204 No Content on success, 404 if not found</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            _logger.LogInformation("DELETE /customers/{CustomerId}", id);

            var result = await _customerService.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("Cannot delete - Customer with ID {CustomerId} not found", id);
                return NotFound(new { message = $"Customer with ID '{id}' was not found." });
            }

            return NoContent();
        }


    }
}
