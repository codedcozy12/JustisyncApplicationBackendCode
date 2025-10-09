using Application.Dto;

namespace Application.Interfaces.Service
{
    public interface ICasePartyService
    {
        Task<ResponseDto<CasePartyCreateDto>> AddCasePartyAsync(CasePartyCreateDto newParty, Guid createdBy);
        Task<ResponseDto<CasePartyUpdateDto>> UpdateCasePartyAsync(CasePartyUpdateDto updatedParty, Guid modifiedBy);
        Task<ResponseDto<CasePartyDto?>> GetCasePartyByIdAsync(Guid casePartyId);
        Task<ResponseDto<IEnumerable<CasePartyDto>>> GetCasePartiesByCaseIdAsync(Guid caseId);
    }
}
