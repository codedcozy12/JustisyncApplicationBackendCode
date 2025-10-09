using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CasePartiesController : ControllerBase
    {
        private readonly ICasePartyService _casePartyService;
        private readonly ILogger<CasePartiesController> _logger;
        public CasePartiesController(ICasePartyService casePartyService, ILogger<CasePartiesController> logger)
        {
            _casePartyService = casePartyService;
            _logger = logger;
        }
        [HttpPost]
        [SwaggerOperation(Summary = "Add case party", Description = "Adds a new party to a specific case.")]
        public async Task<IActionResult> AddCaseParty([FromBody] CasePartyCreateDto newParty, [FromQuery] Guid createdBy)
        {
            var result = await _casePartyService.AddCasePartyAsync(newParty, createdBy);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(
                nameof(GetCasePartyById),
                new { caseId = result.Data!.CaseId },
                result
            );
        }
        [HttpGet("case/{caseId:guid}")]
        [SwaggerOperation(Summary = "Get case parties by case ID", Description = "Retrieves all parties associated with a specific case.")]
        public async Task<IActionResult> GetCasePartiesByCaseId(Guid caseId)
        {
            var result = await _casePartyService.GetCasePartiesByCaseIdAsync(caseId);
            return Ok(result);
        }
        [HttpGet("{casePartyId:guid}")]
        [SwaggerOperation(Summary = "Get case party by ID", Description = "Retrieves the details of a specific case party by its unique identifier.")]
        public async Task<IActionResult> GetCasePartyById(Guid casePartyId)
        {
            var result = await _casePartyService.GetCasePartyByIdAsync(casePartyId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        [HttpPut("update")]
        [SwaggerOperation(Summary = "Update case party", Description = "Updates the details of an existing case party.")]
        public async Task<IActionResult> UpdateCaseParty([FromBody] CasePartyUpdateDto updatedParty, Guid modifiedBy)
        {
            var result = await _casePartyService.UpdateCasePartyAsync(updatedParty, modifiedBy);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
