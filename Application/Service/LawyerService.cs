using System.Text.Encodings.Web;
using Application.Dto;
using Application.Interface;
using Application.Interfaces.ExternalService;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Application.IService;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class LawyerService : ILawyerService
    {
        private readonly ILawyerRepository _lawyerRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LawyerService> _logger;
        private readonly IUploadService _uploadService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IAuditLogService _auditLogService;
        public LawyerService(ILawyerRepository lawyerRepository, IUnitOfWork unitOfWork, ILogger<LawyerService> logger, IUploadService uploadService, IEmailService emailService, ICacheService cacheService,IUserRepository userRepository, ICertificateRepository certificateRepository, IAuditLogService auditLogService)
        {
            _lawyerRepository = lawyerRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _uploadService = uploadService;
            _emailService = emailService;
            _userRepository = userRepository;
            _certificateRepository = certificateRepository;
            _cacheService = cacheService;
            _auditLogService = auditLogService;
        }
        public async Task<ResponseDto<Guid>> CreateLawyerAsync(LawyerCreateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Lawyer creation request is null");
                return ResponseDto<Guid>.Fail("request is null");
            }
            if(request.CertificateTypes.Count != request.CertificateFiles.Count)
            {
                _logger.LogWarning("Lawyer creation request is null");
                return ResponseDto<Guid>.Fail("request is null");
            }
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Lawyer with Email: {Email} already exists", request.Email);
                return ResponseDto<Guid>.Fail("Lawyer with Email: {Email} already exists");
            }
            var user = new User
            {
                Email = request.Email,
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                PhoneNumber = request.PhoneNumber,
                Role = Role.lawyer,
                Accountstatus = Accountstatus.Active
            };
            if (request.ProfilePictureUrl != null)
            {
                var profilePicturePath = _uploadService.UploadProfile(request.ProfilePictureUrl);
                user.ProfilePictureUrl = profilePicturePath;
            }

            var lawyer = new Lawyer
            {
                UserId = user.Id,
                LawyerSpecialization = request.LawyerSpecialization,
                UniversityName = request.UniversityName,
                BarMembershipNumber = request.BarMembershipNumber,
                LawSchoolName = request.LawSchoolName,
                VerificationStatus = lawyerVerificationStatus.pending,
            };

            for(int i = 0;i < request.CertificateFiles.Count; i++)
            {
                var certificateUrl = _uploadService.UploadFile(request.CertificateFiles[i]);
                if (certificateUrl == null)
                {
                    continue;
                }
                var certificate = new Certificate
                {
                    CertificateType = request.CertificateTypes[i],
                    FilePath = certificateUrl,
                    LawyerId = lawyer.Id
                };
                lawyer.Certificates.Add(certificate);
            }
            await _userRepository.AddAsync(user);
            await _lawyerRepository.AddAsync(lawyer);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Lawyer with UserId: {UserId} created successfully", request);
            await _auditLogService.LogActionAsync(user.Id, "Create", "Lawyer", lawyer.Id, $"Lawyer {user.FirstName} {user.LastName} created with ID {lawyer.Id}", Role.lawyer.ToString());
            await SendWelcomeMessageToLawyerAsync(user.Email, user.FirstName);
            var data = await SendEmailConfirmationCode(request.Email);
            _logger.LogInformation("Email Sent To Registered Lawyer");
            await _auditLogService.LogActionAsync(user.Id, "Email", "Lawyer", lawyer.Id, $"Lawyer {user.FirstName} {user.LastName} email sent to {user.Email}", Role.lawyer.ToString());
            return ResponseDto<Guid>.Success(user.Id,"Lawyer Registration Successful");
        }
        public async Task<bool> DeleteLawyerAsync(Guid lawyerId)
        {
            var lawyer = await _lawyerRepository.GetByIdAsync(lawyerId);
            if (lawyer == null)
            {
                _logger.LogWarning("Lawyer with Id: {LawyerId} not found", lawyerId);
                return false;
            }
            var user = await _userRepository.GetByIdAsync(lawyerId);
            if (user == null) {
                _logger.LogWarning("User with Id: {LawyerId} not found", lawyerId);
                return false;
            }
            lawyer.IsDeleted = true;
            await _lawyerRepository.UpdateAsync(lawyer);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Lawyer with Id: {LawyerId} deleted successfully", lawyerId);
            return true;
        }
        public async Task<ResponseDto<IEnumerable<LawyerDto>>> GetAllLawyersAsync()
        {
            var lawyers = await _lawyerRepository.GetAllAsync();
            if (lawyers == null || !lawyers.Any())
            {
                _logger.LogInformation("No lawyers found");
                return new ResponseDto<IEnumerable<LawyerDto>>
                {
                    Data = Enumerable.Empty<LawyerDto>(),
                    IsSuccess = true,
                    Message = "No lawyers found"
                };
            }
            var lawyerDtos = lawyers.Select(l => new LawyerDto
            {
                UserId = l.UserId,
                FirstName = l.User.FirstName,
                LastName = l.User.LastName,
                MiddleName = l.User.MiddleName,
                PhoneNumber = l.User.PhoneNumber,
                ProfilePictureUrl = l.User.ProfilePictureUrl,
                LawyerSpecialization = l.LawyerSpecialization,
                UniversityName = l.UniversityName,
                BarMembershipNumber = l.BarMembershipNumber,
                LawSchoolName = l.LawSchoolName,
                Username = l.User?.Username ?? "Unknown",
                VerificationStatus = l.VerificationStatus,
                Certificates = l.Certificates?.Select(c => new CertificateDto
                {
                    FilePath = c.FilePath,
                    CertificateType = c.CertificateType,
                    Id = c.Id,
                    LawyerId = c.LawyerId,
                    LawyerName = l.User != null ? $"{l.User.FirstName} {l.User.LastName}" : "Unknown",
                    UploadedAt = c.UploadedAt
                }).ToList() ?? new List<CertificateDto>(),
                Id = l.Id,
                Email = l.User?.Email ?? "Unknown"
            }).ToList();
            return new ResponseDto<IEnumerable<LawyerDto>>
            {
                Data = lawyerDtos,
                IsSuccess = true,
                Message = "Lawyers retrieved successfully"
            };
        }
        public async Task<ResponseDto<IEnumerable<LawyerDto>>> GetVerifiedLawyersAsync()
        {
            var lawyers = await _lawyerRepository.GetVerifiedLawyersAsync();
            if (lawyers == null || !lawyers.Any())
            {
                _logger.LogInformation("No verified lawyers found");
                return new ResponseDto<IEnumerable<LawyerDto>>
                {
                    Data = Enumerable.Empty<LawyerDto>(),
                    IsSuccess = true,
                    Message = "No verified lawyers found"
                };
            }
            var lawyerDtos = lawyers.Select(l => new LawyerDto
            {
                FirstName = l.User.FirstName,
                LastName = l.User.LastName,
                MiddleName = l.User.MiddleName,
                PhoneNumber = l.User.PhoneNumber,
                Email = l.User.Email,
                Certificates = l.Certificates.Select(a => new CertificateDto
                {
                    CertificateType = a.CertificateType,
                    FilePath = a.FilePath,
                    Id = a.Id,
                    LawyerId = a.LawyerId,
                    LawyerName = a.Lawyer.User.FirstName,
                    UploadedAt = a.UploadedAt
                }).ToList(),
                ProfilePictureUrl = l.User.ProfilePictureUrl,
                Id = l.Id,
                UserId = l.UserId,
                LawyerSpecialization = l.LawyerSpecialization,
                UniversityName = l.UniversityName,
                BarMembershipNumber = l.BarMembershipNumber,
                LawSchoolName = l.LawSchoolName,
                Username = l.User?.Username ?? "Unknown",
                VerificationStatus = l.VerificationStatus,

            }).ToList();
            return new ResponseDto<IEnumerable<LawyerDto>>
            {
                Data = lawyerDtos,
                IsSuccess = true,
                Message = "Verified lawyers retrieved successfully"
            };
        }
        public async Task<ResponseDto<LawyerDto>> GetCertificatesByLawyerAsync(Guid lawyerId)
        {
            var lawyer = await _lawyerRepository.GetByIdAsync(lawyerId);

            if (lawyer == null)
            {
                _logger.LogWarning("Lawyer with Id: {LawyerId} not found", lawyerId);
                return new ResponseDto<LawyerDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Lawyer not found"
                };
            }
            var lawyerDto = new LawyerDto
            {
                FirstName = lawyer.User.FirstName,
                LastName = lawyer.User.LastName,
                MiddleName = lawyer.User.MiddleName,
                PhoneNumber = lawyer.User.PhoneNumber,
                ProfilePictureUrl = lawyer.User.ProfilePictureUrl,
                UserId = lawyer.UserId,
                LawyerSpecialization = lawyer.LawyerSpecialization,
                UniversityName = lawyer.UniversityName,
                BarMembershipNumber = lawyer.BarMembershipNumber,
                LawSchoolName = lawyer.LawSchoolName,
                Username = lawyer.User?.Username ?? "Unknown",
                VerificationStatus = lawyer.VerificationStatus,
                Certificates = lawyer.Certificates?.Select(c => new CertificateDto
                {
                    FilePath = c.FilePath,
                    CertificateType = c.CertificateType,
                    Id = c.Id,
                    LawyerId = c.LawyerId,
                    LawyerName = lawyer.User != null ? $"{lawyer.User.FirstName} {lawyer.User.LastName}" : "Unknown",
                    UploadedAt = c.UploadedAt
                }).ToList() ?? new List<CertificateDto>(),
                Id = lawyer.Id,
                Email = lawyer.User?.Email ?? "Unknown"
            };

            return new ResponseDto<LawyerDto>
            {
                Data = lawyerDto,
                IsSuccess = true,
                Message = "Certificates retrieved successfully"
            };
        }
        public async Task<ResponseDto<LawyerDto>> GetLawyerByIdAsync(Guid lawyerId)
        {
            var lawyer = await _lawyerRepository.GetByIdAsync(lawyerId);
            if (lawyer == null)
            {
                _logger.LogWarning("Lawyer with Id: {LawyerId} not found", lawyerId);
                return new ResponseDto<LawyerDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Lawyer not found"
                };
            }
            var lawyerDto = new LawyerDto
            {
                FirstName = lawyer.User.FirstName,
                LastName = lawyer.User.LastName,
                MiddleName = lawyer.User.MiddleName,
                PhoneNumber = lawyer.User.PhoneNumber,
                ProfilePictureUrl = lawyer.User.ProfilePictureUrl,
                UserId = lawyer.UserId,
                LawyerSpecialization = lawyer.LawyerSpecialization,
                UniversityName = lawyer.UniversityName,
                BarMembershipNumber = lawyer.BarMembershipNumber,
                LawSchoolName = lawyer.LawSchoolName,
                Username = lawyer.User?.Username ?? "Unknown",
                VerificationStatus = lawyer.VerificationStatus,
                Certificates = lawyer.Certificates?.Select(c => new CertificateDto
                {
                    FilePath = c.FilePath,
                    CertificateType = c.CertificateType,
                    Id = c.Id,
                    LawyerId = c.LawyerId,
                    LawyerName = lawyer.User != null ? $"{lawyer.User.FirstName} {lawyer.User.LastName}" : "Unknown",
                    UploadedAt = c.UploadedAt
                }).ToList() ?? new List<CertificateDto>(),
                Id = lawyer.Id,
                Email = lawyer.User?.Email ?? "Unknown"
            };
            return new ResponseDto<LawyerDto>
            {
                Data = lawyerDto,
                IsSuccess = true,
                Message = "Lawyer retrieved successfully"
            };
        }
        public async Task<ResponseDto<IEnumerable<LawyerDto>>> SearchLawyersAsync(string? university, lawyerVerificationStatus? verificationStatus, LawyerSpecialization? specialization, PageRequest request)
        {
            var lawyers = await _lawyerRepository.SearchAsync(x => (!specialization.HasValue||x.LawyerSpecialization == specialization.Value) && (!verificationStatus.HasValue || x.VerificationStatus == verificationStatus.Value) && (string.IsNullOrEmpty(university) || x.UniversityName.ToLower().Contains(university.ToLower())) , request);
            if (lawyers == null || !lawyers.Items.Any())
            {
                _logger.LogInformation("No lawyers found for the search criteria");
                return new ResponseDto<IEnumerable<LawyerDto>>
                {
                    Data = Enumerable.Empty<LawyerDto>(),
                    IsSuccess = true,
                    Message = "No lawyers found"
                };
            }
            var lawyerDtos = lawyers.Items.Select(l => new LawyerDto
            {
                FirstName = l.User.FirstName,
                LastName = l.User.LastName,
                MiddleName = l.User.MiddleName,
                PhoneNumber = l.User.PhoneNumber,
                ProfilePictureUrl = l.User.ProfilePictureUrl,
                UserId = l.UserId,
                LawyerSpecialization = l.LawyerSpecialization,
                UniversityName = l.UniversityName,
                BarMembershipNumber = l.BarMembershipNumber,
                LawSchoolName = l.LawSchoolName,
                Username = l.User?.Username ?? "Unknown",
                VerificationStatus = l.VerificationStatus,
                Certificates = l.Certificates?.Select(c => new CertificateDto
                {
                    FilePath = c.FilePath,
                    CertificateType = c.CertificateType,
                    Id = c.Id,
                    LawyerId = c.LawyerId,
                    LawyerName = l.User != null ? $"{l.User.FirstName} {l.User.LastName}" : "Unknown",
                    UploadedAt = c.UploadedAt
                }).ToList() ?? new List<CertificateDto>(),
                Id = l.Id,
                Email = l.User?.Email ?? "Unknown"
            }).ToList();
            return new ResponseDto<IEnumerable<LawyerDto>>
            {
                Data = lawyerDtos,
                IsSuccess = true,
                Message = "Lawyers retrieved successfully"
            };
        }
        public async Task<ResponseDto<string>> UpdateLawyerAsync(Guid lawyerId, LawyerUpdateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Lawyer update request is null");
                return null;
            }
            var lawyer = await _lawyerRepository.GetByIdAsync(lawyerId);
            if (lawyer == null)
            {
                _logger.LogWarning("Lawyer with Id: {LawyerId} not found", lawyerId);
                return null;
            }
            lawyer.LawyerSpecialization = request.LawyerSpecialization;
            lawyer.UniversityName = request.UniversityName;
            lawyer.BarMembershipNumber = request.BarMembershipNumber;
            lawyer.LawSchoolName = request.LawSchoolName;
            await _lawyerRepository.UpdateAsync(lawyer);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Lawyer with Id: {LawyerId} updated successfully", lawyerId);
            await _auditLogService.LogActionAsync(lawyer.UserId, "Update", "Lawyer", lawyer.Id, $"Lawyer {lawyer.User.FirstName} {lawyer.User.LastName} updated with ID {lawyer.Id}", Role.lawyer.ToString());
            return ResponseDto<string>.Success($"Lawyer update is successful");
        }
        public async Task<ResponseDto<string>> UpdateVerificationStatusAsync(Guid lawyerId, LawyerUpdateVerifcationRequestDto request)
        {
            var lawyer = await _lawyerRepository.GetByIdAsync(lawyerId);
            if (lawyer == null)
            {
                _logger.LogWarning("Lawyer with Id: {LawyerId} not found", lawyerId);
                return null;
            }
            lawyer.VerificationStatus = request.VerificationStatus;

            await _lawyerRepository.UpdateAsync(lawyer);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Lawyer with Id: {LawyerId} verification status updated to {Status}", lawyerId, request.VerificationStatus);
            _auditLogService.LogActionAsync(lawyer.UserId, "Update", "Lawyer", lawyer.Id, $"Lawyer {lawyer.User.FirstName} {lawyer.User.LastName} verification status updated to {request.VerificationStatus}", Role.lawyer.ToString());
            return ResponseDto<string>.Success($"Lawyer Verification for {lawyer.User.FirstName} was successful \nStatus {request.VerificationStatus}");
        }
        public async Task<ResponseDto<string>> UploadCertificatesAsync(Guid lawyerId, CertificateUploadRequestDto requests)
        {
            if (requests == null || !requests.Files.Any())
            {
                _logger.LogWarning("Certificate upload request list is null or empty");
                return null;
            }

            var lawyer = await _lawyerRepository.GetByIdAsync(lawyerId);
            if (lawyer == null)
            {
                _logger.LogWarning("Lawyer with Id: {LawyerId} not found", lawyerId);
                return null;
            }

            var uploadedCertificates = new List<Certificate>();

            foreach (var request in requests.Files)
            {
                var filePath = _uploadService.UploadFile(request);
                if (string.IsNullOrEmpty(filePath))
                {
                    _logger.LogError("Failed to upload certificate file for request {CertificateType}", request.ContentType);
                    continue;
                }

                var certificate = new Certificate
                {
                    FilePath = filePath,
                    LawyerId = lawyerId
                };

                uploadedCertificates.Add(certificate);
            }

            for (int i = 0; i < uploadedCertificates.Count; i++)
            {
                uploadedCertificates[i].CertificateType = requests.CertificateType[i];
            }

            if (!uploadedCertificates.Any())
            {
                _logger.LogWarning("No certificates were successfully uploaded for Lawyer with Id: {LawyerId}", lawyerId);
                return null;
            }
            await _certificateRepository.AddRangeAsync(uploadedCertificates);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{Count} certificates uploaded successfully for Lawyer with Id: {LawyerId}", uploadedCertificates.Count, lawyerId);
            return ResponseDto<string>.Success($"{uploadedCertificates.Count} certificates uploaded successfully");
        }
        public async Task<ResponseDto<string>> VerifyCertificateAsync(Guid lawyerId, CertificateUpdateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Certificate verification request is null");
                return null;
            }
            var lawyer = await _lawyerRepository.GetByIdAsync(lawyerId);
            if (lawyer == null)
            {
                return ResponseDto<string>.Fail("Lawyer not found");
            }
            
            var certificates = await _certificateRepository.GetByLawyerIdAsync(lawyerId);
            if (certificates.Count() == 0)
            {
                _logger.LogWarning("Certificate with Id: {CertificateId} not found", lawyerId);
                return ResponseDto<string>.Fail("Certificate Not Found");
            }
            foreach (var certificate in certificates)
            {
                certificate.IsVerified = request.IsVerified;
                certificate.VerifiedByUserId = request.VerifiedByUserId;
            }
            if(request.IsVerified)
                lawyer.VerificationStatus = lawyerVerificationStatus.Verified;
            else
                lawyer.VerificationStatus = lawyerVerificationStatus.Rejected;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Lawyer with Id: {lawyer} verified successfully", lawyerId);
            if (request.IsVerified)
                await _emailService.SendEmailAsync(lawyer.User.Email, "Lawyer Verification Mail", lawyerApprovedEmailHtml);
            else
                await _emailService.SendEmailAsync(lawyer.User.Email, "Lawyer Verification Mail", lawyerRejectedEmailHtml);

            _logger.LogInformation("Verification Of Lawyer Email Sent");

            return ResponseDto<string>.Success("Lawyer Verification Was Successful");
        }

        public async Task<ResponseDto<PageResponse<LawyerDto>>> GetPendingLawyers(PageRequest pageRequest)
        {
            string key = $"GetPendingLawyers{pageRequest.PageSize}:{pageRequest.PageNumber}";
            var cachedLawyers = await _cacheService.GetAsync<PageResponse<LawyerDto>>(key);
            if (cachedLawyers != null)
                return ResponseDto<PageResponse<LawyerDto>>.Success(cachedLawyers);

            var lawyers = await _lawyerRepository.GetPendingLawyersAsync(pageRequest);

            var lawyerDtos = new PageResponse<LawyerDto>
            {
                Items = lawyers.Items.Select(l => new LawyerDto
                {
                    Id = l.Id,
                    FirstName = l.User.FirstName,
                    LastName = l.User.LastName,
                    MiddleName = l.User.MiddleName,
                    PhoneNumber = l.User.PhoneNumber,
                    ProfilePictureUrl = l.User.ProfilePictureUrl,
                    Email = l.User.Email,
                    Certificates = l.Certificates?.Select(c => new CertificateDto
                    {
                        FilePath = c.FilePath,
                        CertificateType = c.CertificateType,
                        Id = c.Id,
                        LawyerId = c.LawyerId,
                        LawyerName = l.User != null ? $"{l.User.FirstName} {l.User.LastName}" : "Unknown",
                        UploadedAt = c.UploadedAt
                    }).ToList() ?? new List<CertificateDto>(),
                    UserId = l.UserId,
                    LawyerSpecialization = l.LawyerSpecialization,
                    UniversityName = l.UniversityName,
                    BarMembershipNumber = l.BarMembershipNumber,
                    LawSchoolName = l.LawSchoolName,
                    Username = l.User?.Username ?? "Unknown",
                    VerificationStatus = l.VerificationStatus
                }).ToList(),
                PageNumber = lawyers.PageNumber,
                PageSize = lawyers.PageSize,
                TotalCount = lawyers.TotalCount
            };
            await _cacheService.AddAsync(key,lawyerDtos,TimeSpan.FromMinutes(10));
            _logger.LogInformation("Pending Lawyers... Retrieved Successfully");
            return ResponseDto<PageResponse<LawyerDto>>.Success(lawyerDtos);
        }
        public async Task<ResponseDto<string>> SendEmailConfirmationCode(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed. Email {Email} not found.", email);
                return ResponseDto<string>.Fail("User not found.");
            }

            string code = new Random().Next(100000, 999999).ToString();
            string cacheKey = $"EmailConfirmationToken:{email}";
            await _cacheService.AddAsync(cacheKey, code, TimeSpan.FromMinutes(15));
            await SendVerificationEmail(email, code, user.FirstName);
            _logger.LogInformation("Email confirmation code sent to email: {Email}", email);
            return ResponseDto<string>.Success("Verification code sent to email.");
        }
        private async Task SendVerificationEmail(string email, string code, string name)
        {
            string emailTitle = "Verify Your Email - JustiSync";

            string body = $@"
<!doctype html>
<html lang=""en"">
  <head>
    <meta charset=""utf-8""/>
    <title>{emailTitle}</title>
  </head>
  <body style=""font-family:Arial,sans-serif;background-color:#f9f9f9;padding:20px;"">
    <div style=""max-width:600px;margin:auto;background:white;padding:30px;border-radius:8px;box-shadow:0 0 10px rgba(0,0,0,0.1);"">
      <h2 style=""color:#0b3d91;text-align:center;"">{emailTitle}</h2>
      <p>Hi {name},</p>
      <p>Thanks for signing up with <b>JustiSync</b>. Please use the code below to verify your email:</p>

      <div style=""text-align:center;margin:20px 0;"">
        <h1 style=""font-size:32px;letter-spacing:5px;color:#0b3d91;"">{code}</h1>
      </div>

      <p>If you didn’t create an account, you can safely ignore this email.</p>
      <br/>
      <p style=""color:#888;"">The JustiSync Team</p>
    </div>
  </body>
</html>";

            await _emailService.SendEmailAsync(email, emailTitle, body);

        }
        private async Task SendWelcomeMessageToLawyerAsync(string email, string name)
        {
            var subject = "Welcome to JustiSyncApplication — Let’s get your practice verified";

            var safeName = HtmlEncoder.Default.Encode(string.IsNullOrWhiteSpace(name) ? "Counsel" : name);

            var dashboardUrl = "https://your-app-url/dashboard";
            
            var uploadCertificatesUrl = "https://your-app-url/profile/certificates";
            
            var supportEmail = "support@justisync.app";

            var html = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Welcome to JustiSyncApplication</title>
  <style>
    /* Basic, inline-friendly styles for email clients */
    body {{ margin:0; padding:0; background:#f6f8fb; font-family:Segoe UI, Roboto, Helvetica, Arial, sans-serif; color:#0f172a; }}
    .container {{ max-width:640px; margin:0 auto; background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 2px 10px rgba(2,6,23,0.06); }}
    .header {{ background:#0ea5e9; color:#ffffff; padding:24px; text-align:center; }}
    .header h1 {{ margin:0; font-size:22px; }}
    .content {{ padding:24px 28px; line-height:1.6; }}
    .cta {{ display:inline-block; padding:12px 18px; background:#0ea5e9; color:#ffffff !important; text-decoration:none; border-radius:8px; font-weight:600; }}
    .muted {{ color:#475569; font-size:14px; }}
    .card {{ background:#f8fafc; border:1px solid #e2e8f0; border-radius:10px; padding:16px; margin:18px 0; }}
    ul {{ padding-left:18px; margin:10px 0; }}
    a {{ color:#0ea5e9; }}
    .footer {{ text-align:center; color:#64748b; font-size:12px; padding:18px 12px; }}
  </style>
</head>
<body>
  <div style=""padding:18px;"">
    <div class=""container"">
      <div class=""header"">
        <h1>Welcome to JustiSyncApplication</h1>
        <div style=""font-size:13px; opacity:0.95;"">Smart tools for modern legal practice</div>
      </div>

      <div class=""content"">
        <p>Hi {safeName},</p>
        <p>We’re excited to have you on board! Your JustiSyncApplication account has been created. To start connecting with clients and courts, please complete the quick steps below.</p>

        <div class=""card"">
          <strong>✅ Next steps to get verified</strong>
          <ul>
            <li>Complete your profile (photo, phone number, specialization).</li>
            <li>Upload your <strong>University</strong> and <strong>Law School</strong> certificates.</li>
            <li>Provide your <strong>Bar Membership Number</strong>.</li>
            <li>Submit for admin verification &mdash; we’ll notify you once approved.</li>
          </ul>
        </div>

        <p style=""margin:20px 0;"">
          <a class=""cta"" href=""{dashboardUrl}"" target=""_blank"">Go to your dashboard</a>
        </p>

        <p class=""muted"">You can upload certificates anytime from your profile:
          <a href=""{uploadCertificatesUrl}"" target=""_blank"">Upload certificates</a>
        </p>

        <div class=""card"">
          <strong>Tips for faster approval</strong>
          <ul>
            <li>Ensure certificates are clear (PDF/JPG/PNG) and readable.</li>
            <li>Make sure your name matches your Bar records.</li>
            <li>Keep your specialization accurate to help clients find you.</li>
          </ul>
        </div>

        <p>If you need any help, reply to this email or contact us at <a href=""mailto:{supportEmail}"">{supportEmail}</a>.</p>

        <p>Welcome aboard,<br/>The JustiSyncApplication Team</p>
      </div>

      <div class=""footer"">
        © {DateTime.UtcNow.Year} JustiSyncApplication. All rights reserved.
      </div>
    </div>
  </div>
</body>
</html>";
            await _emailService.SendEmailAsync(email, subject, html);
        }


        string lawyerApprovedEmailHtml = @"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body { font-family: Arial, sans-serif; color: #333; }
    .container { max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px; }
    .header { font-size: 20px; font-weight: bold; color: #2d6a4f; }
    .btn {
        display: inline-block; 
        margin-top: 20px; 
        padding: 10px 20px; 
        background-color: #2d6a4f; 
        color: #fff; 
        text-decoration: none; 
        border-radius: 5px;
    }
  </style>
</head>
<body>
  <div class='container'>
    <p class='header'>✅ Lawyer Verification Approved</p> 
    <p>Hi,</p>
    <p>We are pleased to inform you that your lawyer verification request has been <b>approved</b> after successfully reviewing your uploaded certificates.</p>
    <p>You are now a <b>verified lawyer</b> on JustiSync 🎉. This grants you access to client connections, case handling, and full platform features.</p>
    <a class='btn' href='https://justisync.com/login'>Login to Your Dashboard</a>
    <p style='margin-top:30px;'>Best regards,<br><b>The JustiSync Team</b></p>
  </div>
</body>
</html>";


        string lawyerRejectedEmailHtml = @"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body { font-family: Arial, sans-serif; color: #333; }
    .container { max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px; }
    .header { font-size: 20px; font-weight: bold; color: #d00000; }
    .btn {
        display: inline-block; 
        margin-top: 20px; 
        padding: 10px 20px; 
        background-color: #d00000; 
        color: #fff; 
        text-decoration: none; 
        border-radius: 5px;
    }
  </style>
</head>
<body>
  <div class='container'>
    <p class='header'>⚠️ Lawyer Verification Rejected</p>
    <p>Hello,</p>
    <p>After reviewing your uploaded certificates, we regret to inform you that your lawyer verification request has been <b>rejected</b>.</p>
    <p>This may be due to one or more of the following reasons:</p>
    <ul>
      <li>Incomplete or missing certificates</li>
      <li>Invalid or unverifiable document(s)</li>
      <li>Mismatched information with your profile</li>
    </ul>
    <p>You may reapply by uploading valid certificates for verification:</p>
    <a class='btn' href='https://justisync.com/reapply-verification'>Reapply for Verification</a>
    <p style='margin-top:30px;'>If you believe this is a mistake, kindly contact our support team.</p>
    <p>Best regards,<br><b>The JustiSync Team</b></p>
  </div>
</body>
</html>";
    }
}