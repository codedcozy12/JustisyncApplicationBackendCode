using Application.Dto;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiversion}/[controller]")]
    public class JurisdictionsController : Controller
    {
        private readonly IJurisdictionService _jurisdictionService;
        private readonly ICourtService _courtService;
        private readonly ILogger<JurisdictionsController> _logger;
        public JurisdictionsController(IJurisdictionService jurisdictionService, ICourtService courtService, ILogger<JurisdictionsController> logger)
        {
            _jurisdictionService = jurisdictionService;
            _courtService = courtService;
            _logger = logger;
        }
        [Authorize]
        [HttpGet]
        [SwaggerOperation(Summary = "Get all jurisdictions", Description = "Retrieve a list of all jurisdictions in the system.")]
        public async Task<IActionResult> GetAllJurisdictions()
        {
            var result = await _jurisdictionService.GetAllAsync();
            return Ok(result);
        }
        [HttpGet("{jurisdictionId:guid}")]
        [SwaggerOperation(Summary = "Get jurisdiction by Id", Description = "Retrieve details of a specific jurisdiction by its unique Id.")]
        public async Task<IActionResult> GetJurisdictionById(Guid jurisdictionId)
        {
            var result = await _jurisdictionService.GetByIdAsync(jurisdictionId);
            if (result == null || !result.IsSuccess)
            {
                return NotFound(result?.Message ?? "Jurisdiction not found.");
            }
            return Ok(result);
        }
        [Authorize]
        [HttpGet("by-subject-matter")]
        [SwaggerOperation(Summary = "Get jurisdictions by subject matter", Description = "Retrieve jurisdictions that match a specific subject matter.")]
        public async Task<IActionResult> GetJurisdictionsBySubjectMatter([FromQuery] string subjectMatter)
        {
            var result = await _jurisdictionService.GetBySubjectMatterAsync(subjectMatter);
            return Ok(result);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new jurisdiction", Description = "Add a new jurisdiction to the system.")]
        public async Task<IActionResult> CreateJurisdiction([FromBody] JurisdictionCreateRequestDto request)
        {
            var result = await _jurisdictionService.CreateAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{jurisdictionId:guid}")]
        [SwaggerOperation(Summary = "Update jurisdiction details", Description = "Update the information of an existing jurisdiction.")]
        public async Task<IActionResult> UpdateJurisdiction(Guid jurisdictionId, [FromBody] JurisdictionUpdateRequestDto request)
        {
            if (jurisdictionId != request.Id)
            {
                return BadRequest("Jurisdiction ID mismatch.");
            }
            var result = await _jurisdictionService.UpdateAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [Authorize(Roles = "admin")]
        [HttpPost("add-to-court/{courtId:guid}")]
        [SwaggerOperation(Summary = "Add jurisdictions to a court", Description = "Associate one or more jurisdictions with a specific court.")]
        public async Task<IActionResult> AddJurisdictionToCourt(List<Guid> Ids, Guid courtId)
        {
            var result = await _jurisdictionService.AddJurisdictionToCourt(Ids, courtId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [Authorize]
        [HttpGet("by-court/{courtId:guid}")]
            [SwaggerOperation(Summary = "Get jurisdictions by court", Description = "Retrieve a list of jurisdictions associated with a specific court.")]
            public async Task<IActionResult> GetJurisdictionsByCourt(Guid courtId)
            {
                var court = await _courtService.GetCourtByIdAsync(courtId);
                if (court == null || !court.IsSuccess)
                {
                    return NotFound(court?.Message ?? "Court not found.");
                }
                var result = await _jurisdictionService.GetByCourtIdAsync(courtId);
                return Ok(result);
            }
        }
    }