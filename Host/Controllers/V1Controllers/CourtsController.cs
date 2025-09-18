using Application.Dto;
using Application.Interfaces.AuthService;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CourtsController : ControllerBase
    {
        private readonly ICourtService _courtService;
        private readonly ILogger<CourtsController> _logger;
        private readonly IAuthService _authService;
        public CourtsController(ICourtService courtService, ILogger<CourtsController> logger, IAuthService authService)
        {
            _courtService = courtService;
            _logger = logger;
            _authService = authService;
        }
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new court", Description = "Registers a new court in the system.")]
        public async Task<IActionResult> CreateCourt([FromForm] CourtCreateRequestDto request)
        {
            var result = await _courtService.CreateCourtAsync(request);
            return Ok(result);
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Get all courts", Description = "Retrieves a list of all courts in the system.")]
        public async Task<IActionResult> GetAllCourts()
        {
            var result = await _courtService.GetAllCourtsAsync();
            return Ok(result);
        }
        [HttpGet("active")]
        [SwaggerOperation(Summary = "Get all active courts", Description = "Retrieves a list of all active courts in the system.")]
        public async Task<IActionResult> GetActiveCourts()
        {
            var result = await _courtService.GetActiveCourtsAsync();
            return Ok(result);
        }
        [HttpGet("{courtId:guid}")]
        [SwaggerOperation(Summary = "Get court by Id", Description = "Retrieves details of a court by their unique Id.")]
        public async Task<IActionResult> GetCourtById(Guid courtId)
        {
            var result = await _courtService.GetCourtByIdAsync(courtId);
            return Ok(result);
        }
        [HttpPut("{courtId:guid}")]
        [SwaggerOperation(Summary = "Update court details", Description = "Updates the information of an existing court.")]
        public async Task<IActionResult> UpdateCourt(Guid courtId, [FromBody] CourtUpdateRequestDto request)
        {
            var result = await _courtService.UpdateCourtAsync(courtId, request);
            return Ok(result);
        }
        [HttpDelete("{courtId:guid}")]
        [SwaggerOperation(Summary = "Delete court", Description = "Deletes a court by their unique Id.")]
        public async Task<IActionResult> DeleteCourt(Guid courtId)
        {
            var success = await _courtService.DeleteCourtAsync(courtId);
            if (!success) return NotFound("Court not found or already deleted.");
            return Ok("Court deleted successfully.");
        }
    }
}