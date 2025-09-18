using Application.Dto;

namespace Application.Interfaces.Service
{
    public interface ICertificateService
    {
        Task<CertificateCreateRequestDto> UploadCertificateAsync(Guid lawyerId, CertificateCreateRequestDto request);
        Task<ResponseDto<CertificateDto>> GetCertificateByIdAsync(Guid certificateId);
        Task<ResponseDto<IEnumerable<CertificateDto>>> GetAllCertificatesAsync();
        Task<CertificateUpdateRequestDto> VerifyCertificateAsync(Guid certificateId, CertificateUpdateRequestDto request);
        Task<ResponseDto<IEnumerable<CertificateDto>>> GetCertificatesByLawyerAsync(Guid lawyerId);
        Task<bool> DeleteCertificateAsync(Guid certificateId);
        //Task<ResponseDto<IEnumerable<CertificateDto>>> SearchCertificatesAsync(string? fileName, bool? isVerified, PageRequest request);
    }
}