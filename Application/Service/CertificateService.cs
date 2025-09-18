using Application.Dto;
using Application.Interfaces.ExternalService;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILawyerRepository _lawyerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CertificateService> _logger;
        private readonly ICacheService _cacheService;
        private readonly IUploadService _uploadService;
        private readonly IEmailService _emailService;
        public CertificateService(ICertificateRepository certificateRepository, IUnitOfWork unitOfWork, ILogger<CertificateService> logger, ICacheService cacheService, IUploadService uploadService, IEmailService emailService)
        {
            _certificateRepository = certificateRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
            _uploadService = uploadService;
            _emailService = emailService;
        }
        public async Task<bool> DeleteCertificateAsync(Guid certificateId)
        {
            var certificate = await _certificateRepository.GetByIdAsync(certificateId);
            if (certificate == null)
            {
                _logger.LogWarning("Certificate with Id: {CertificateId} not found", certificateId);
                return false;
            }
            await _certificateRepository.DeleteAsync(certificateId);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Certificate with Id: {CertificateId} deleted successfully", certificateId);
            return true;
        }
        public async Task<ResponseDto<IEnumerable<CertificateDto>>> GetAllCertificatesAsync()
        {
            var certificates = await _certificateRepository.GetAllAsync();
            if (certificates == null || !certificates.Any())
            {
                _logger.LogInformation("No certificates found");
                return new ResponseDto<IEnumerable<CertificateDto>>
                {
                    Data = Enumerable.Empty<CertificateDto>(),
                    IsSuccess = true,
                    Message = "No certificates found"
                };
            }
            var certificateDtos = certificates.Select(c => new CertificateDto
            {
                Id = c.Id,
                LawyerId = c.LawyerId,
                LawyerName = c.Lawyer?.User?.Username ?? "Unknown",
                CertificateType = c.CertificateType
            });
            return new ResponseDto<IEnumerable<CertificateDto>>
            {
                Data = certificateDtos,
                IsSuccess = true,
                Message = "Certificates retrieved successfully"
            };
        }
        public async Task<ResponseDto<CertificateDto>> GetCertificateByIdAsync(Guid certificateId)
        {
            var certificate = await _certificateRepository.GetByIdAsync(certificateId);
            if (certificate == null)
            {
                _logger.LogWarning("Certificate with Id: {CertificateId} not found", certificateId);
                return new ResponseDto<CertificateDto>
                {
                    IsSuccess = false,
                    Message = "Certificate not found"
                };
            }
            var certificateDto = new CertificateDto
            {
                Id = certificate.Id,
                LawyerId = certificate.LawyerId,
                LawyerName = certificate.Lawyer?.User?.Username ?? "Unknown",
                CertificateType = certificate.CertificateType
            };
            return new ResponseDto<CertificateDto>
            {
                Data = certificateDto,
                IsSuccess = true,
                Message = "Certificate retrieved successfully"
            };
        }
        public async Task<ResponseDto<IEnumerable<CertificateDto>>> GetCertificatesByLawyerAsync(Guid lawyerId)
        {
            var certificates = await _certificateRepository.GetByLawyerIdAsync(lawyerId);
            if (certificates == null || !certificates.Any())
            {
                _logger.LogInformation("No certificates found for Lawyer with Id: {LawyerId}", lawyerId);
                return new ResponseDto<IEnumerable<CertificateDto>>
                {
                    Data = Enumerable.Empty<CertificateDto>(),
                    IsSuccess = true,
                    Message = "No certificates found for the specified lawyer"
                };
            }
            var certificateDtos = certificates.Select(c => new CertificateDto
            {
                Id = c.Id,
                LawyerId = c.LawyerId,
                LawyerName = c.Lawyer?.User?.Username ?? "Unknown",
                CertificateType = c.CertificateType
            });
            return new ResponseDto<IEnumerable<CertificateDto>>
            {
                Data = certificateDtos,
                IsSuccess = true,
                Message = "Certificates retrieved successfully"
            };
        }
        public async Task<CertificateCreateRequestDto> UploadCertificateAsync(Guid lawyerId, CertificateCreateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Certificate upload request is null");
                return null;
            }
            var lawyer = await _lawyerRepository.GetByIdAsync(lawyerId);
            if (lawyer == null)
            {
                _logger.LogWarning("Lawyer with Id: {LawyerId} not found", lawyerId);
                return null;
            }
            var path = _uploadService.UploadFile(request.File);
            if (path == null)
            {
                _logger.LogWarning("Certificate with Id: {CertificateId} Error uploading certificate", lawyerId);
                return null;
            }
            var certificate = new Certificate
            {
                LawyerId = lawyerId,
                FilePath = path,
                CertificateType = request.CertificateType,
                IsVerified = false
            };
            await _certificateRepository.AddAsync(certificate);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Certificate uploaded successfully for Lawyer with Id: {LawyerId}", lawyerId);
            return request;
        }
        public async Task<CertificateUpdateRequestDto> VerifyCertificateAsync(Guid certificateId, CertificateUpdateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Certificate verification request is null");
                return null;
            }
            var certificate = await _certificateRepository.GetByIdAsync(certificateId);
            if (certificate == null)
            {
                _logger.LogWarning("Certificate with Id: {CertificateId} not found", certificateId);
                return null;
            }
            certificate.IsVerified = request.IsVerified;
            await _certificateRepository.UpdateAsync(certificate);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Certificate with Id: {CertificateId} verified successfully", certificateId);
            return request;
        }
    }
}