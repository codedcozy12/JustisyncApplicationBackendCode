using Application.Dto;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CaseProceedingRecordsController : ControllerBase
    {
        private readonly ILogger<CaseProceedingRecordsController> _logger;
        private readonly ICaseProceedingRecordService _caseProceedingRecordService;
        public CaseProceedingRecordsController(ILogger<CaseProceedingRecordsController> logger, ICaseProceedingRecordService caseProceedingRecordService)
        {
            _logger = logger;
            _caseProceedingRecordService = caseProceedingRecordService;
        }
        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create case proceeding record", Description = "Creates a new proceeding record for a specific case.")]
        public async Task<IActionResult> CreateCaseProceedingRecord([FromBody] CaseProceedingRecordCreateDto newRecord, [FromQuery] Guid createdBy)
        {
            var result = await _caseProceedingRecordService.CreateAsync(newRecord, createdBy);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(
                nameof(GetCaseProceedingRecordsByCaseId),
                new { caseId = result.Data!.CaseId },
                result
            );
        }
        [HttpGet("case/{caseId:guid}")]
        [SwaggerOperation(Summary = "Get case proceeding records by case ID", Description = "Retrieves all proceeding records associated with a specific case.")]
        public async Task<IActionResult> GetCaseProceedingRecordsByCaseId(Guid caseId)
        {
            var result = await _caseProceedingRecordService.GetByCaseIdAsync(caseId);
            return Ok(result);
        }
        [HttpGet("{proceedingId:guid}")]
        [SwaggerOperation(Summary = "Get case proceeding record by ID", Description = "Retrieves the details of a specific case proceeding record by its unique identifier.")]
        public async Task<IActionResult> GetCaseProceedingRecordById(Guid proceedingId)
        {
            var result = await _caseProceedingRecordService.GetByIdAsync(proceedingId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        [HttpPut("update")]
        [SwaggerOperation(Summary = "Update case proceeding record", Description = "Updates the details of an existing case proceeding record.")]
        public async Task<IActionResult> UpdateCaseProceedingRecord([FromBody] CaseProceedingRecordUpdateDto updatedRecord, [FromQuery] Guid modifiedBy)
        {
            var result = await _caseProceedingRecordService.UpdateAsync(updatedRecord, modifiedBy);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}