using Application.Dto;
using Domain.Enums;

namespace Application.IService
{
    public interface ILawyerService
    {
        Task<ResponseDto<Guid>> CreateLawyerAsync(LawyerCreateRequestDto request);
        Task<ResponseDto<LawyerDto>> GetLawyerByIdAsync(Guid lawyerId);
        Task<ResponseDto<IEnumerable<LawyerDto>>> GetAllLawyersAsync();
        Task<bool> DeleteLawyerAsync(Guid lawyerId);
        Task<ResponseDto<string>> UploadCertificatesAsync(Guid lawyerId, CertificateUploadRequestDto requests);
        Task<ResponseDto<PageResponse<LawyerDto>>> GetPendingLawyers(PageRequest pageRequest);
        Task<ResponseDto<IEnumerable<LawyerDto>>> GetVerifiedLawyersAsync();
        Task<ResponseDto<string>> VerifyCertificateAsync(Guid lawyerId, CertificateUpdateRequestDto request);
        Task<ResponseDto<LawyerDto>> GetCertificatesByLawyerAsync(Guid lawyerId);
        Task<ResponseDto<string>> SendEmailConfirmationCode(string email);
        Task<ResponseDto<string>> UpdateVerificationStatusAsync(Guid lawyerId, LawyerUpdateVerifcationRequestDto status);
        Task<ResponseDto<IEnumerable<LawyerDto>>> SearchLawyersAsync(string? university, lawyerVerificationStatus? verificationStatus, LawyerSpecialization? specialization, PageRequest request);
    }
}