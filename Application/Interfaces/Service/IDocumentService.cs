using Application.Dto;

namespace Application.Interfaces.Service
{
    public interface IDocumentService
    {
        Task<ResponseDto<DocumentDto?>> GetDocumentByIdAsync(Guid documentId);
        Task<ResponseDto<IEnumerable<DocumentDto>>> GetDocumentsByCaseIdAsync(Guid caseId);
        Task<ResponseDto<DocumentCreateRequestDto>> CreateDocumentAsync(Guid lawyerId, DocumentCreateRequestDto newDocument);
    }
}