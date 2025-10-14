using Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CasesController : ControllerBase
    {
        private readonly ICaseService _caseService;
        private readonly ILogger<CasesController> _logger;
        public CasesController(ICaseService caseService, ILogger<CasesController> logger)
        {
            _caseService = caseService;
            _logger = logger;
        }

        [Authorize(Roles = "client")]
        [HttpPost("initiate")]
        [SwaggerOperation(Summary = "Initiate case filing", Description = "Initiates the filing of a new case for a specific client.")]
        public async Task<IActionResult> InitiateCaseFiling([FromBody] CaseCreateRequestDto newCase)
        {
            var result = await _caseService.InitiateCaseFilingAsync(newCase, newCase.ClientId);
            return CreatedAtAction(nameof(GetCaseById), new { caseId = result.Data?.Id }, result);
        }

        [Authorize]
        [HttpPut("assign/{caseId:guid}")]
        [SwaggerOperation(Summary = "Assign judge and schedule case", Description = "Assigns a judge to the case and schedules it for a specific date.")]
        public async Task<IActionResult> AssignJudgeAndSchedule(Guid caseId, [FromQuery] Guid registrarId, [FromQuery] Guid judgeId, [FromQuery] DateTime scheduledDate)
        {
            var result = await _caseService.AssignJudgeAndScheduleAsync(caseId, registrarId, judgeId, scheduledDate);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("{caseId:guid}")]
        [SwaggerOperation(Summary = "Get case by ID", Description = "Retrieves the details of a specific case by its unique identifier.")]
        public async Task<IActionResult> GetCaseById(Guid caseId)
        {
            var result = await _caseService.GetCaseByIdAsync(caseId);
            if (result.Data == null)
                return NotFound(result);

            return Ok(result);
        }
        [Authorize(Roles = "lawyer")]
        [HttpPut("complete/{caseId:guid}")]
        [SwaggerOperation(Summary = "Complete case filing", Description = "Completes the filing of a case by adding necessary documents and descriptions.")]
        public async Task<IActionResult> CompleteCaseFiling(Guid caseId, [FromQuery] Guid lawyerId, [FromQuery]Guid courtId, [FromForm] DocumentCreateRequestDto document, string Description)
        {
            var result = await _caseService.CompleteCaseFilingAsync(caseId, lawyerId,courtId, Description, document);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("client/{clientId:guid}")]
        [SwaggerOperation(Summary = "Get cases by client", Description = "Retrieves all cases associated with a specific client.")]
        public async Task<IActionResult> GetCasesByClient(Guid clientId)
        {
            var result = await _caseService.GetCasesByClientAsync(clientId);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("lawyer/{lawyerId:guid}")]
        [SwaggerOperation(Summary = "Get cases by lawyer", Description = "Retrieves all cases associated with a specific lawyer.")]
        public async Task<IActionResult> GetCasesByLawyer(Guid lawyerId)
        {
            var result = await _caseService.GetCasesByLawyerAsync(lawyerId);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("judge/{judgeId:guid}")]
        [SwaggerOperation(Summary = "Get cases by judge", Description = "Retrieves all cases associated with a specific judge.")]
        public async Task<IActionResult> GetCasesByJudge(Guid judgeId)
        {
            var result = await _caseService.GetCasesByJudgeAsync(judgeId);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("pending")]
        [SwaggerOperation(Summary = "Get pending cases", Description = "Retrieves all cases that are currently pending.")]
        public async Task<IActionResult> GetPendingCases()
        {
            var result = await _caseService.GetPendingCasesAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Registrar")]
        [HttpPut("verify")]
        [SwaggerOperation(Summary = "Verify case jurisdictions", Description = "Verifies the jurisdiction attached to a specific case.")]
        public async Task<IActionResult> VerifyCaseJurisdiction([FromBody] CaseUpdateVerificationRequestDto request)
        {
            if (request == null || request.CaseId == Guid.Empty)
            {
                return BadRequest("Invalid request data.");
            }

            var result = await _caseService.VerifyCaseJurisdictionAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpGet("closed")]
        [SwaggerOperation(Summary = "Get closed cases", Description = "Retrieves all cases that have been closed.")]
        public async Task<IActionResult> GetClosedCases()
        {
            var result = await _caseService.GetClosedCasesAsync();
            return Ok(result);
        }
        [Authorize]
        [HttpPost("notify/{caseId:guid}")]
        [SwaggerOperation(Summary = "Notify case parties", Description = "Sends a notification message to all parties involved in a specific case.")]
        public async Task<IActionResult> NotifyCaseParties(Guid caseId, [FromQuery] string message)
        {
            await _caseService.NotifyCasePartiesAsync(caseId, message);
            return NoContent();
        }
    }
}