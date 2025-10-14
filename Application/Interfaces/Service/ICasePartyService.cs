using Application.Dto;

namespace Application.Interfaces.Service
{
    public interface ICasePartyService
    {
        //Task<ResponseDto<string>> CheckIfCaseExistsAsync(Guid caseId);
        Task<ResponseDto<CasePartyCreateDto>> AddCasePartyAsync(CasePartyCreateDto newParty);
        Task<ResponseDto<CasePartyUpdateDto>> UpdateCasePartyAsync(CasePartyUpdateDto updatedParty, Guid modifiedBy);
        Task<ResponseDto<CasePartyDto?>> GetCasePartyByIdAsync(Guid casePartyId);
        Task<ResponseDto<IEnumerable<CasePartyDto>>> GetCasePartiesByCaseIdAsync(Guid caseId);
    }
}
