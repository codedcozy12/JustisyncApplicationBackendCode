using Application.Dto;
using Application.Interfaces.AuthService;
using Application.Interfaces.Service;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class JudgesController : ControllerBase
    {
        private readonly IJudgeService _judgeService;
        private readonly ILogger<JudgesController> _logger;
        private readonly IAuthService _authService;
        public JudgesController(IJudgeService judgeService, ILogger<JudgesController> logger, IAuthService authService)
        {
            _judgeService = judgeService;
            _logger = logger;
            _authService = authService;
        }
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new judge", Description = "Registers a new judge in the system.")]
        public async Task<IActionResult> CreateJudge([FromForm] JudgeCreateRequestDto request)
        {
            var result = await _judgeService.CreateJudgeAsync(request);
            return Ok(result);
        }
        [Authorize(Roles = "admin,registrar")]
        [HttpGet("{judgeId:guid}")]
        [SwaggerOperation(Summary = "Get judge by Id", Description = "Retrieves details of a judge by their unique Id.")]
        public async Task<IActionResult> GetJudgeById(Guid judgeId)
        {
            var result = await _judgeService.GetJudgeByIdAsync(judgeId);
            return Ok(result);
        }
        [Authorize(Roles = "admin,registrar")]
        [HttpGet]
        [SwaggerOperation(Summary = "Get all judges", Description = "Retrieves a list of all judges in the system.")]
        public async Task<IActionResult> GetAllJudges()
        {
            var result = await _judgeService.GetAllJudgesAsync();
            return Ok(result);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{judgeId:guid}")]
        [SwaggerOperation(Summary = "Update judge details", Description = "Updates the information of an existing judge.")]
        public async Task<IActionResult> UpdateJudge(Guid judgeId, [FromBody] JudgeUpdateRequestDto request)
        {
            var result = await _judgeService.UpdateJudgeAsync(judgeId, request);
            return Ok(result);
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{judgeId:guid}")]
        [SwaggerOperation(Summary = "Delete judge", Description = "Deletes a judge by their unique Id.")]
        public async Task<IActionResult> DeleteJudge(Guid judgeId)
        {
            var success = await _judgeService.DeleteJudgeAsync(judgeId);
            if (!success) return NotFound("Judge not found or already deleted.");
            return Ok("Judge deleted successfully.");
        }

        [Authorize(Roles = "admin,registrar")]
        [HttpGet("check-judge/{courtId:guid}")]
        [SwaggerOperation( Summary = "Check if a court has an assigned judge", Description = "Verifies whether a specific court currently has a judge assigned to it."
)]
        public async Task<IActionResult> CheckIfCourtHasJudge(Guid courtId)
        {
            if (courtId == Guid.Empty)
                return BadRequest("Invalid court ID.");

            var result = await _judgeService.CheckIfCourtHasJudgeAsync(courtId);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("by-court/{courtId:guid}")]
        [SwaggerOperation(Summary = "Get judges by court", Description = "Retrieves a list of judges assigned to a specific court.")]
        public async Task<IActionResult> GetJudgesByCourt(Guid courtId)
        {
            var result = await _judgeService.GetJudgesByCourtAsync(courtId);
            return Ok(result);
        }
        [Authorize(Roles = "registrar")]
        [HttpPost("assign-to-court")]
        [SwaggerOperation(Summary = "Assign judge to a court", Description = "Assigns a judge to a specified court with an optional appointment date.")]
        public async Task<IActionResult> AssignJudgeToCourt(AssignJudgeRequestDto dto)
        {
            var result = await _judgeService.AssignJudgeToCourtAsync(dto);
            return Ok(result);
        }
    }
}