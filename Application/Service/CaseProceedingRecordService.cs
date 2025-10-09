using Application.Dto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class CaseProceedingRecordService : ICaseProceedingRecordService
    {
        private readonly ICaseProceedingRecordRepository _caseProceedingRecordRepository;
        private readonly ICaseRepository _caseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CaseProceedingRecordService> _logger;

        public CaseProceedingRecordService(
            ICaseProceedingRecordRepository caseProceedingRecordRepository,
            ICaseRepository caseRepository,
            ILogger<CaseProceedingRecordService> logger, IUnitOfWork unitOfWork)
        {
            _caseProceedingRecordRepository = caseProceedingRecordRepository;
            _caseRepository = caseRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDto<CaseProceedingRecordCreateDto>> CreateAsync(CaseProceedingRecordCreateDto newRecord, Guid createdBy)
        {
            var caseEntity = await _caseRepository.GetCaseByIdAsync(newRecord.CaseId);
            if (caseEntity == null)
                return ResponseDto<CaseProceedingRecordCreateDto>.Fail("Case not found.");
            var proceeding = new CaseProceedingRecord
            {
                Id = Guid.NewGuid(),
                Title = newRecord.Description,
                CaseId = newRecord.CaseId,
                CourtSessionId = newRecord.CourtSessionId,
                JudgeId = newRecord.JudgeId,
                Notes = newRecord.Notes,
                ScheduledDate = newRecord.ProceedingDate,
                Description = newRecord.Description,
                Outcome = newRecord.Outcome ?? string.Empty
            };
            await _caseProceedingRecordRepository.AddProceedingAsync(proceeding);
            await _unitOfWork.SaveChangesAsync();

            return ResponseDto<CaseProceedingRecordCreateDto>.Success(newRecord, "Proceeding record created successfully.");
        }
        public async Task<ResponseDto<IEnumerable<CaseProceedingRecordDto>>> GetByCaseIdAsync(Guid caseId)
        {
            var records = await _caseProceedingRecordRepository.GetProceedingsByCaseAsync(caseId);
            if (records == null || !records.Any())
                return ResponseDto<IEnumerable<CaseProceedingRecordDto>>.Fail("No proceeding records found for this case.");

            var dtos = records.Select(r => new CaseProceedingRecordDto
            {
                Id = r.Id,
                CaseId = r.CaseId,
                JudgeId = r.Id,
                CourtSessionId = r.CourtSessionId,
                Notes = r.Notes,
                ProceedingDate = r.ScheduledDate,
                Description = r.Description,
                Outcome = r.Outcome
            }).ToList();

            return ResponseDto<IEnumerable<CaseProceedingRecordDto>>.Success(dtos, "Proceeding records retrieved successfully.");
        }
        public async Task<ResponseDto<CaseProceedingRecordDto?>> GetByIdAsync(Guid proceedingId)
        {
            var record = await _caseProceedingRecordRepository.GetProceedingByIdAsync(proceedingId);
            if (record == null)
                return ResponseDto<CaseProceedingRecordDto?>.Fail("Proceeding record not found.");

            var dto = new CaseProceedingRecordDto
            {
                Id = record.Id,
                CaseId = record.CaseId,
                CourtSessionId = record.CourtSessionId,
                Notes = record.Notes,
                Description = record.Description,
                Outcome = record.Outcome,
                ProceedingDate = record.ScheduledDate
            };

            return ResponseDto<CaseProceedingRecordDto?>.Success(dto, "Record retrieved successfully.");
        }
        public async Task<ResponseDto<CaseProceedingRecordUpdateDto>> UpdateAsync(CaseProceedingRecordUpdateDto updatedRecord, Guid modifiedBy)
        {
            var record = await _caseProceedingRecordRepository.GetProceedingByIdAsync(updatedRecord.Id);
            if (record == null)
                return ResponseDto<CaseProceedingRecordUpdateDto>.Fail("Proceeding record not found.");

            record.Title = updatedRecord.Description ?? record.Title;
            record.Notes = updatedRecord.Notes ?? record.Notes;
            record.ScheduledDate = updatedRecord.ProceedingDate ?? record.ScheduledDate;
            record.DateModified = DateTime.UtcNow;
            record.ModifiedBy = updatedRecord.JudgeId.ToString();
            await _caseProceedingRecordRepository.UpdateProceedingAsync(record);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Proceeding record {RecordId} updated by user {ModifiedBy}", record.Id, modifiedBy);

            return ResponseDto<CaseProceedingRecordUpdateDto>.Success(updatedRecord, "Proceeding record updated successfully.");
        }
    }
}