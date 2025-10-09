using Application.Dto;

namespace Application.Interfaces.Service
{
    public interface ICaseProceedingRecordService
    {
        Task<ResponseDto<CaseProceedingRecordDto?>> GetByIdAsync(Guid proceedingId);
        Task<ResponseDto<IEnumerable<CaseProceedingRecordDto>>> GetByCaseIdAsync(Guid caseId);
        Task<ResponseDto<CaseProceedingRecordCreateDto>> CreateAsync(CaseProceedingRecordCreateDto newRecord, Guid createdBy);
        Task<ResponseDto<CaseProceedingRecordUpdateDto>> UpdateAsync(CaseProceedingRecordUpdateDto updatedRecord, Guid modifiedBy);
    }
}
