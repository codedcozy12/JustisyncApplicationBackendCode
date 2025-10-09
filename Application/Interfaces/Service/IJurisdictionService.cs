using Application.Dto;

namespace Application.Interfaces.Service
{
    public interface IJurisdictionService
    {
        Task<IEnumerable<ResponseDto<JurisdictionDto>>> GetAllAsync();
        Task<ResponseDto<JurisdictionDto>> GetByIdAsync(Guid id);
        Task<IEnumerable<ResponseDto<JurisdictionDto>>> GetBySubjectMatterAsync(string subjectMatter);
        Task<IEnumerable<ResponseDto<JurisdictionDto>>> GetByCourtIdAsync(Guid courtId);
        Task<ResponseDto<Guid>> CreateAsync(JurisdictionCreateRequestDto dto);
        Task<ResponseDto<Guid>> UpdateAsync(JurisdictionUpdateRequestDto dto);
        Task<ResponseDto<Guid>> AddJurisdictionToCourt(List<Guid> Ids,Guid courtId);
        Task<bool> DeleteAsync(Guid id);
    }
}