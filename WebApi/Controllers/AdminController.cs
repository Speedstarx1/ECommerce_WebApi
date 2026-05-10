using Application.Dtos.RequestDto;
using Application.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new Admin (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] AdminCreateDto request)
        {
            var createdBy = User.FindFirstValue(ClaimTypes.Email) ?? "System";
            _logger.LogInformation("POST /admin - Creating Admin with email {AdminEmail} ", request.Email);

            var admin = await _adminService.CreateAsync(request, createdBy);

            return CreatedAtAction(nameof(GetById), new { id = admin.Id, version = "1.0" }, admin);
        }

        /// <summary>
        /// Get admin by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound(new { message = $"Admin with ID '{id}' was not found." });
            }
            return Ok(admin);
        }

        /// <summary>
        /// Get all admin members (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var adminList = await _adminService.GetAllAsync();
            return Ok(adminList);
        }

        

    }
}