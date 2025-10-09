using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentService _documentService;
        public DocumentsController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDocumentById(Guid id)
        {
            var result = await _documentService.GetDocumentByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("case/{caseId:guid}")]
        public async Task<IActionResult> GetDocumentsByCaseId(Guid caseId)
        {
            var result = await _documentService.GetDocumentsByCaseIdAsync(caseId);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] DocumentCreateRequestDto newdocument, Guid lawyerId)
        {
            var result = await _documentService.CreateDocumentAsync(lawyerId,newdocument);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(
                nameof(GetDocumentById),
                new { id = result.Data!.CaseId },
                result
            );
        }
    }
}