using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces.Service
{
    public interface IJudgeService
    {
        Task<ResponseDto<JudgeCreateRequestDto>> CreateJudgeAsync(JudgeCreateRequestDto request);
        Task<ResponseDto<JudgeDto>> GetJudgeByIdAsync(Guid judgeId);
        Task<ResponseDto<IEnumerable<JudgeDto>>> GetAllJudgesAsync();
        Task<JudgeUpdateRequestDto> UpdateJudgeAsync(Guid judgeId, JudgeUpdateRequestDto request);
        Task<bool> DeleteJudgeAsync(Guid judgeId);
        Task<ResponseDto<IEnumerable<JudgeDto>>> GetJudgesByCourtAsync(Guid courtId);
        Task<JudgeUpdateRequestDto> AssignJudgeToCourtAsync(Guid judgeId, Guid courtId, DateTime? appointmentDate);
    }
}