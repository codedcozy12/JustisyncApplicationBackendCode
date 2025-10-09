using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Document?> GetDocumentByIdAsync(Guid documentId)
    {
        return await _context.Documents
            .Include(d => d.Case)
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted);
    }

    public async Task AddDocumentAsync(Document newDocument)
    {
        await _context.Documents.AddAsync(newDocument);
    }
    public async Task<IEnumerable<Document>> GetDocumentsByCaseIdAsync(Guid caseId)
    {
        return await _context.Documents
            .Where(d => d.CaseId == caseId && !d.IsDeleted)
            .ToListAsync();
    }
}