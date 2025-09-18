using Application.Dto;

namespace Application.Interfaces.Service
{
    public interface ICourtService
    {
        Task<ResponseDto<CourtCreateRequestDto>> CreateCourtAsync(CourtCreateRequestDto request);
        Task<ResponseDto<CourtDto>> GetCourtByIdAsync(Guid courtId);
        Task<ResponseDto<IEnumerable<CourtDto>>> GetAllCourtsAsync();
        Task<ResponseDto<IEnumerable<CourtDto>>> GetActiveCourtsAsync();
        Task<CourtUpdateRequestDto> UpdateCourtAsync(Guid courtId, CourtUpdateRequestDto request);
        Task<bool> DeleteCourtAsync(Guid courtId);
    }
}
