using Application.Dto;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CourtSessionsController : ControllerBase
    {
        private readonly ILogger<CourtSessionsController> _logger;
        private readonly ICourtSessionService _courtSessionService;
        public CourtSessionsController(ILogger<CourtSessionsController> logger, ICourtSessionService courtSessionService)
        {
            _logger = logger;
            _courtSessionService = courtSessionService;
        }
        [HttpGet("case/{caseId:guid}")]
        [SwaggerOperation(Summary = "Get court sessions by case ID", Description = "Retrieves all court sessions associated with a specific case.")]
        public async Task<IActionResult> GetByCaseId(Guid caseId)
        {
            var result = await _courtSessionService.GetByCaseIdAsync(caseId);
            return Ok(result);
        }
        [HttpGet("court/{courtId:guid}")]
        [SwaggerOperation(Summary = "Get court sessions by court ID", Description = "Retrieves all court sessions associated with a specific court.")]
        public async Task<IActionResult> GetByCourtId(Guid courtId)
        {
            var result = await _courtSessionService.GetByCourtIdAsync(courtId);
            return Ok(result);
        }
        [HttpGet("{sessionId:guid}")]
        [SwaggerOperation(Summary = "Get court session by ID", Description = "Retrieves the details of a specific court session by its unique identifier.")]
        public async Task<IActionResult> GetById(Guid sessionId)
        {
            var result = await _courtSessionService.GetByIdAsync(sessionId);
            if (result.Data == null)
                return NotFound(result);
            return Ok(result);
        }
        [HttpGet("upcoming")]
        [SwaggerOperation(Summary = "Get upcoming court sessions", Description = "Retrieves upcoming court sessions between a specified date range.")]
        public async Task<IActionResult> GetUpcomingSessions([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from == default || to == default)
                return BadRequest("Invalid date range.");

            var result = await _courtSessionService.GetUpcomingSessionsAsync(from, to);
            return Ok(result);
        }
        [HttpPut("{id:guid}")]
        [SwaggerOperation(Summary = "Update court session", Description = "Allows registrar or authorized personnel to update court session details.")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourtSessionUpdateRequestDto dto)
        {
            if (id == Guid.Empty || dto == null)
                return BadRequest("Invalid request data.");

            var result = await _courtSessionService.UpdateAsync(id, dto);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}