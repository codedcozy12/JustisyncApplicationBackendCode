using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IDocumentRepository
    {
        Task<Document?> GetDocumentByIdAsync(Guid documentId);
        Task AddDocumentAsync(Document newDocument);
        Task<IEnumerable<Document>> GetDocumentsByCaseIdAsync(Guid caseId);
    }
}