using Application.Dto;
using Application.Interfaces.AuthService;
using Application.IService;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LawyersController : ControllerBase
    {
        private readonly ILawyerService _lawyerService;
        private readonly ILogger<LawyersController> _logger;
        private readonly IAuthService _authService;

        public LawyersController(ILawyerService lawyerService, ILogger<LawyersController> logger, IAuthService authService)
        {
            _lawyerService = lawyerService;
            _logger = logger;
            _authService = authService;
        }
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new lawyer", Description = "Registers a new lawyer in the system using the provided details.")]
        public async Task<ActionResult<LawyerCreateRequestDto?>> CreateLawyer([FromForm] LawyerCreateRequestDto request)
        {
            var result = await _lawyerService.CreateLawyerAsync(request);
            return Ok(result);
        }
        [HttpGet("{lawyerId:guid}")]
        [SwaggerOperation(
            Summary = "Get lawyer by ID",
            Description = "Retrieves detailed information of a specific lawyer by their unique identifier."
        )]
        public async Task<ActionResult<ResponseDto<LawyerDto>>> GetLawyerById(Guid lawyerId)
        {
            var result = await _lawyerService.GetLawyerByIdAsync(lawyerId);
            return Ok(result);
        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all lawyers",
            Description = "Fetches a list of all lawyers registered in the system."
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<LawyerDto>>>> GetAllLawyers()
        {
            var result = await _lawyerService.GetAllLawyersAsync();
            return Ok(result);
        }
        [HttpPut("{lawyerId:guid}")]
        [SwaggerOperation(
            Summary = "Update lawyer details",
            Description = "Updates profile information of a specific lawyer."
        )]
        public async Task<ActionResult<LawyerUpdateRequestDto>> UpdateLawyer(Guid lawyerId, [FromBody] LawyerUpdateRequestDto request)
        {
            var result = await _lawyerService.UpdateLawyerAsync(lawyerId, request);
            return Ok(result);
        }
        [HttpDelete("{lawyerId:guid}")]
        [SwaggerOperation(
            Summary = "Delete a lawyer",
            Description = "Removes a lawyer account from the system by their unique identifier."
        )]
        public async Task<ActionResult<bool>> DeleteLawyer(Guid lawyerId)
        {
            var result = await _lawyerService.DeleteLawyerAsync(lawyerId);
            return Ok(result);
        }
        [HttpPost("{lawyerId:guid}/certificates")]
        [SwaggerOperation(
            Summary = "Upload certificate for lawyer",
            Description = "Uploads a certificate document for a specific lawyer for verification purposes."
        )]
        public async Task<ActionResult<CertificateCreateRequestDto>> UploadCertificate(Guid lawyerId, [FromForm] CertificateCreateRequestDto request)
        {
            var result = await _lawyerService.UploadCertificateAsync(lawyerId, request);
            return Ok(result);
        }
        [HttpGet("verified")]
        [SwaggerOperation(
            Summary = "Get verified lawyers",
            Description = "Returns a list of lawyers whose profiles have been verified."
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<LawyerDto>>>> GetVerifiedLawyers()
        {
            var result = await _lawyerService.GetVerifiedLawyersAsync();
            return Ok(result);
        }


        [HttpGet("pending")]
        [SwaggerOperation(
            Summary = "Get pending lawyers",
            Description = "Returns a list of lawyers whose profiles have not been verified."
        )]
        public async Task<ActionResult<ResponseDto<PageResponse<LawyerDto>>>> GetPendingLawyers([FromQuery]PageRequest pageRequest)
        {
            var result = await _lawyerService.GetPendingLawyers(pageRequest);
            return Ok(result);
        }

        [HttpPut("certificates/{lawyerId:guid}/verify")]
        [SwaggerOperation(
            Summary = "Verify certificate",
            Description = "Marks a lawyer’s certificates as verified or rejected based on review."
        )]
        public async Task<ActionResult<CertificateUpdateRequestDto>> VerifyCertificates(Guid lawyerId, [FromBody] CertificateUpdateRequestDto request)
        {
            var result = await _lawyerService.VerifyCertificateAsync(lawyerId, request);
            return Ok(result);
        }

        [HttpGet("{lawyerId:guid}/certificates")]
        [SwaggerOperation(
            Summary = "Get certificates of a lawyer",
            Description = "Retrieves all uploaded certificates of a given lawyer."
        )]
        public async Task<ActionResult<ResponseDto<LawyerDto>>> GetCertificatesByLawyer(Guid lawyerId)
        {
            var result = await _lawyerService.GetCertificatesByLawyerAsync(lawyerId);
            return Ok(result);
        }
        [HttpPut("{lawyerId:guid}/verification-status")]
        [SwaggerOperation(
           Summary = "Update lawyer verification status",
           Description = "Updates the verification status of a lawyer’s account (e.g., Pending, Verified, Rejected)."
       )]
        public async Task<ActionResult<LawyerUpdateRequestDto>> UpdateVerificationStatus(Guid lawyerId, [FromBody] LawyerUpdateRequestDto status)
        {
            var result = await _lawyerService.UpdateVerificationStatusAsync(lawyerId, status);
            return Ok(result);
        }
        [HttpGet("search")]
        [SwaggerOperation(
          Summary = "Search lawyers",
          Description = "Searches for lawyers by university, verification status, specialization, and pagination options."
      )]
        public async Task<ActionResult<ResponseDto<IEnumerable<LawyerDto>>>> SearchLawyers(
          [FromQuery] string? university,
          [FromQuery] lawyerVerificationStatus? verificationStatus,
          [FromQuery] LawyerSpecialization? specialization,
          [FromQuery] PageRequest request)
        {
            var result = await _lawyerService.SearchLawyersAsync(university, verificationStatus, specialization, request);
            return Ok(result);
        }
    }
}