using Application.Dto;
using Application.Interfaces.AuthService;
using Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RegistrarsController : ControllerBase
    {
        private readonly IRegistrarService _registrarService;
        private readonly ILogger<RegistrarsController> _logger;
        private readonly IAuthService _authService;
        public RegistrarsController(IRegistrarService registrarService, ILogger<RegistrarsController> logger, IAuthService authService)
        {
            _registrarService = registrarService;
            _logger = logger;
            _authService = authService;
        }
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new registrar", Description = "Registers a new registrar in the system.")]
        public async Task<IActionResult> CreateRegistrar([FromForm] RegistrarCreateRequestDto request)
        {
            var result = await _registrarService.CreateRegistrarAsync(request);
            return Ok(result);
        }
        [HttpGet("{registrarId:guid}")]
        [SwaggerOperation(Summary = "Get registrar by Id", Description = "Retrieves details of a registrar by their unique Id.")]
        public async Task<IActionResult> GetRegistrarById(Guid registrarId)
        {
            var result = await _registrarService.GetRegistrarByIdAsync(registrarId);
            return Ok(result);
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Get all registrars", Description = "Retrieves a list of all registrars in the system.")]
        public async Task<IActionResult> GetAllRegistrars()
        {
            var result = await _registrarService.GetAllRegistrarsAsync();
            return Ok(result);
        }
        [HttpPut("{registrarId:guid}")]
        [SwaggerOperation(Summary = "Update registrar details", Description = "Updates the information of an existing registrar.")]
        public async Task<IActionResult> UpdateRegistrar(Guid registrarId, [FromBody] RegistrarUpdateRequestDto request)
        {
            var result = await _registrarService.UpdateRegistrarAsync(registrarId, request);
            return Ok(result);
        }
        [HttpDelete("{registrarId:guid}")]
        [SwaggerOperation(Summary = "Delete registrar", Description = "Deletes a registrar by their unique Id.")]
        public async Task<IActionResult> DeleteRegistrar(Guid registrarId)
        {
            var success = await _registrarService.DeleteRegistrarAsync(registrarId);
            if (!success) return NotFound("Registrar not found or already deleted.");
            return Ok("Registrar deleted successfully.");
        }
        [HttpGet("by-court/{courtId:guid}")]
        [SwaggerOperation(Summary = "Get registrars by court", Description = "Retrieves a list of registrars assigned to a specific court.")]
        public async Task<IActionResult> GetRegistrarsByCourt(Guid courtId)
        {
            var result = await _registrarService.GetRegistrarsByCourtAsync(courtId);
            return Ok(result);
        }
        [HttpPost("{registrarId:guid}/assign-to-court/{courtId:guid}")]
        [SwaggerOperation(Summary = "Assign registrar to a court", Description = "Assigns a registrar to a specified court with an optional appointment date.")]
        public async Task<IActionResult> AssignRegistrarToCourt(Guid registrarId, Guid courtId, [FromQuery] DateTime? appointmentDate)
        {
            var result = await _registrarService.AssignRegistrarToCourtAsync(registrarId, courtId, appointmentDate);
            return Ok(result);
        }
    }
}