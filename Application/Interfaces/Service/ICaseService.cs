using Application.Dto;

public interface ICaseService
{
    Task<ResponseDto<CaseDto>> InitiateCaseFilingAsync(CaseCreateRequestDto newCase, Guid clientId);
    Task<ResponseDto<CaseDto>> CompleteCaseFilingAsync(Guid caseId, Guid lawyerId,Guid courtId, string Description, DocumentCreateRequestDto document);
    Task<ResponseDto<CaseDto>> AssignJudgeAndScheduleAsync(Guid caseId, Guid registrarId, Guid judgeId, DateTime scheduledDate);
    Task<ResponseDto<CaseDto?>> GetCaseByIdAsync(Guid caseId);
    Task<ResponseDto<IEnumerable<CaseDto>>> GetCasesByClientAsync(Guid clientId);
    Task<ResponseDto<IEnumerable<CaseDto>>> GetCasesByLawyerAsync(Guid lawyerId);
    Task<ResponseDto<IEnumerable<CaseDto>>> GetCasesByJudgeAsync(Guid judgeId);
    Task<ResponseDto<IEnumerable<CaseDto>>> GetPendingCasesAsync();
    Task<ResponseDto<IEnumerable<CaseDto>>> GetClosedCasesAsync();
    Task NotifyCasePartiesAsync(Guid caseId, string message);
}