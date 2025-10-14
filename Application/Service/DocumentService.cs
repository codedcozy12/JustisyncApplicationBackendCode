using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces.ExternalService;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUploadService _uploadService;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(IDocumentRepository documentRepository, IUnitOfWork unitOfWork, ILogger<DocumentService> logger, IUploadService uploadService)
    {
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _uploadService = uploadService;
    }

    public async Task<ResponseDto<DocumentDto?>> GetDocumentByIdAsync(Guid documentId)
    {
        var doc = await _documentRepository.GetDocumentByIdAsync(documentId);
        if (doc == null || doc.IsDeleted)
        {
            _logger.LogWarning("Document with ID {DocumentId} not found", documentId);
            return new ResponseDto<DocumentDto?>
            {
                IsSuccess = false,
                Message = "Document not found",
                Data = null
            };
        }

        var dto = new DocumentDto
        {
            Id = doc.Id,
            CaseId = doc.CaseId,
            FileName = doc.FileName,
            FileType = doc.FileType,
            FileUrl = doc.FileUrl
        };

        return new ResponseDto<DocumentDto?>
        {
            IsSuccess = true,
            Message = "Document retrieved successfully",
            Data = dto
        };
    }

    public async Task<ResponseDto<IEnumerable<DocumentDto>>> GetDocumentsByCaseIdAsync(Guid caseId)
    {
        var docs = await _documentRepository.GetDocumentsByCaseIdAsync(caseId);
        var filteredDocs = docs.Where(d => !d.IsDeleted);
        if (filteredDocs == null || !filteredDocs.Any())
        {
            _logger.LogWarning("No documents found for Case ID {CaseId}", caseId);
            return new ResponseDto<IEnumerable<DocumentDto>>
            {
                IsSuccess = false,
                Message = "No documents found for this case",
                Data = null
            };
        }
        var dtoList = new List<DocumentDto>();

        foreach (var doc in docs)
        {
            dtoList.Add(new DocumentDto
            {
                Id = doc.Id,
                CaseId = doc.CaseId,
                FileName = doc.FileName,
                FileType = doc.FileType,
                FileUrl = doc.FileUrl
            });
        }
        return new ResponseDto<IEnumerable<DocumentDto>>
        {
            IsSuccess = true,
            Message = "Documents retrieved successfully",
            Data = dtoList
        };
    }

    public async Task<ResponseDto<DocumentCreateRequestDto>> CreateDocumentAsync(Guid lawyerId, DocumentCreateRequestDto newDocument)
    {
        var path = _uploadService.UploadDocument(newDocument.FileUrl);
        if (path == null)
        {
            _logger.LogWarning("Document with Id: {DocumentId} Error uploading Documents", lawyerId);
            return null;
        }
        var docEntity = new Document
        {
            CaseId = newDocument.CaseId,
            FileName = newDocument.FileName,
            FileType = newDocument.FileType,
            FileUrl = path,
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
        };

        await _documentRepository.AddDocumentAsync(docEntity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Document {FileName} created for Case ID {CaseId} by {User}",
            newDocument.FileName, newDocument.CaseId);
        return new ResponseDto<DocumentCreateRequestDto>
        {
            IsSuccess = true,
            Message = "Court created successfully",
            Data = newDocument
        };
    }
}