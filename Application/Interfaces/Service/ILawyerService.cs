using Application.Dto;
using Domain.Enums;

namespace Application.IService
{
    public interface ILawyerService
    {
        Task<ResponseDto<Guid>> CreateLawyerAsync(LawyerCreateRequestDto request);
        Task<ResponseDto<LawyerDto>> GetLawyerByIdAsync(Guid lawyerId);
        Task<ResponseDto<IEnumerable<LawyerDto>>> GetAllLawyersAsync();
        Task<LawyerUpdateRequestDto> UpdateLawyerAsync(Guid lawyerId, LawyerUpdateRequestDto request);
        Task<bool> DeleteLawyerAsync(Guid lawyerId);
        Task<CertificateCreateRequestDto> UploadCertificateAsync(Guid lawyerId, CertificateCreateRequestDto request);
        Task<ResponseDto<PageResponse<LawyerDto>>> GetPendingLawyers(PageRequest pageRequest);
        Task<ResponseDto<IEnumerable<LawyerDto>>> GetVerifiedLawyersAsync();
        Task<ResponseDto<string>> VerifyCertificateAsync(Guid lawyerId, CertificateUpdateRequestDto request);
        Task<ResponseDto<LawyerDto>> GetCertificatesByLawyerAsync(Guid lawyerId);
        Task<LawyerUpdateRequestDto> UpdateVerificationStatusAsync(Guid lawyerId, LawyerUpdateRequestDto status);
        Task<ResponseDto<IEnumerable<LawyerDto>>> SearchLawyersAsync(string? university, lawyerVerificationStatus? verificationStatus, LawyerSpecialization? specialization, PageRequest request);
    }
}