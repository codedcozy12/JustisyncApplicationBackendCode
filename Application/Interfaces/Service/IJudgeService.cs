using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces.Service
{
    public interface IJudgeService
    {
        Task<ResponseDto<Guid>> CreateJudgeAsync(JudgeCreateRequestDto request);
        Task<ResponseDto<JudgeDto>> GetJudgeByIdAsync(Guid judgeId);
        Task<ResponseDto<IEnumerable<JudgeDto>>> GetAllJudgesAsync();
        Task<JudgeUpdateRequestDto> UpdateJudgeAsync(Guid judgeId, JudgeUpdateRequestDto request);
        Task<bool> DeleteJudgeAsync(Guid judgeId);
        Task<ResponseDto<IEnumerable<JudgeDto>>> GetJudgesByCourtAsync(Guid courtId);
        Task<ResponseDto<string>> SendEmailConfirmationCode(string email);
        Task<JudgeUpdateRequestDto> AssignJudgeToCourtAsync(AssignJudgeRequestDto dto);
    }
}