using Application.Dto;
using Domain.Enums;

namespace Application.Interfaces.Service
{
public interface ICourtSessionService
    {
        Task<ResponseDto<CourtSessionUpdateRequestDto>> UpdateAsync(Guid id, CourtSessionUpdateRequestDto dto);
        Task<ResponseDto<CourtSessionDto>> GetByIdAsync(Guid id);
        Task<ResponseDto<IEnumerable<CourtSessionDto>>> GetByCaseIdAsync(Guid caseId);
        Task<ResponseDto<IEnumerable<CourtSessionDto>>> GetByCourtIdAsync(Guid courtId);
        Task<ResponseDto<IEnumerable<CourtSessionDto>>> GetUpcomingSessionsAsync(DateTime from, DateTime to);
        Task<ResponseDto<CourtSessionDto>> StartVirtualSessionAsync(Guid sessionId);
        Task<ResponseDto<CourtSessionDto>> EndCourtSessionAsync(Guid sessionId);
    }
}