using System.Reflection.Metadata;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces.ExternalService;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Document = Domain.Entities.Document;

namespace Application.Service
{
    public class CaseService : ICaseService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly ICasePartyRepository _casePartyRepository;
        private readonly ICaseProceedingRecordRepository _proceedingRepository;
        private readonly IJudgeRepository _judgeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IUploadService _uploadService;
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentService _documentService;
        private readonly ILawyerRepository _lawyerRepository;
        private readonly ILogger<CaseService> _logger;
        private readonly IAuditLogService _auditLogService;

        public CaseService(ICaseRepository caseRepository, ICasePartyRepository casePartyRepository, ICaseProceedingRecordRepository proceedingRepository, INotificationService notificationService, ILogger<CaseService> logger, IUnitOfWork unitOfWork, IAuditLogService auditLogService, ILawyerRepository lawyerRepository, IDocumentService documentService, IDocumentRepository documentRepository, IUploadService uploadService, IEmailService emailService, IJudgeRepository judgeRepository)
        {
            _caseRepository = caseRepository;
            _casePartyRepository = casePartyRepository;
            _proceedingRepository = proceedingRepository;
            _notificationService = notificationService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _lawyerRepository = lawyerRepository;
            _documentService = documentService;
            _documentRepository = documentRepository;
            _uploadService = uploadService;
            _emailService = emailService;
            _judgeRepository = judgeRepository;
        }
        public async Task<ResponseDto<CaseDto>> AssignJudgeAndScheduleAsync(Guid caseId, Guid registrarId, Guid judgeId, DateTime scheduledDate)
        {
            _logger.LogInformation("Registrar {RegistrarId} is assigning Judge {JudgeId} to Case {CaseId} with hearing date {ScheduledDate}",
                registrarId, judgeId, caseId, scheduledDate);

            var caseEntity = await _caseRepository.GetCaseByIdAsync(caseId);

            foreach (var party in caseEntity.CaseParties)
            {
                await SendMailToCasePartyAsync(party.ContactEmail, party.FullName, caseEntity.CaseNumber,caseEntity.ModifiedBy,scheduledDate);
            }
            if (caseEntity == null)
            {
                _logger.LogWarning("Case with ID {CaseId} not found", caseId);
                return new ResponseDto<CaseDto>
                {
                    IsSuccess = false,
                    Message = "Case not found."
                };
            }

            caseEntity.JudgeId = judgeId;
            caseEntity.Status = CaseStatus.Scheduled;
            caseEntity.DateModified = DateTime.UtcNow;

            await _caseRepository.UpdateCaseAsync(caseEntity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Judge {JudgeId} successfully assigned to Case {CaseId}", judgeId, caseId);

            var SendMailJudge = await _judgeRepository.GetByIdAsync(judgeId);
                {

                await SendMailToJudgeAsync(SendMailJudge.User.Email, SendMailJudge.User.FirstName, caseEntity.CaseNumber, SendMailJudge.Court.Name, scheduledDate);
            }

            var responseData = new CaseDto
            {
                Id = caseEntity.Id,
                CaseNumber = caseEntity.CaseNumber,
                ClientId = caseEntity.ClientId,
                LawyerId = caseEntity.LawyerId,
                CourtId = caseEntity.CourtId,
                JudgeId = caseEntity.JudgeId,
                OffenseType = caseEntity.OffenseType,
                Description = caseEntity.Description,
                Status = caseEntity.Status,
                CreatedAt = caseEntity.DateCreated,
                UpdatedAt = caseEntity.DateModified
            };

            return new ResponseDto<CaseDto>
            {
                IsSuccess = true,
                Message = "Judge assigned and case scheduled successfully.",
                Data = responseData
            };
        }
        public async Task<ResponseDto<CaseDto>> CompleteCaseFilingAsync(Guid caseId, Guid lawyerId, string Description, DocumentCreateRequestDto document)
        {
            var caseEntity = await _caseRepository.GetCaseByIdAsync(caseId);
            if (caseEntity == null)
                return new ResponseDto<CaseDto> { IsSuccess = false, Message = "Case not found." };

            string path = "";
            if (document.FileUrl != null)
            {
                path = _uploadService.UploadDocument(document.FileUrl);
            }

            await _documentRepository.AddDocumentAsync(new Document
            {
                CaseId = caseId,
                FileName = document.FileName,
                FileType = document.FileType,
                FileUrl = path,
            });

            caseEntity.Status = CaseStatus.Filed;
            caseEntity.LawyerId = lawyerId;
            caseEntity.Description = Description;
            caseEntity.DateModified = DateTime.UtcNow;

            await _caseRepository.UpdateCaseAsync(caseEntity);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDto<CaseDto>
            {
                IsSuccess = true,
                Message = "Case filing completed successfully.",
                Data = new CaseDto
                {
                    ClientId = caseEntity.ClientId,
                    CaseNumber = caseEntity.CaseNumber,
                    Description = Description
                }
            };
        }
        public async Task<ResponseDto<CaseDto?>> GetCaseByIdAsync(Guid caseId)
        {
            var caseEntity = await _caseRepository.GetCaseByIdAsync(caseId);
            if (caseEntity == null)
                return new ResponseDto<CaseDto?> { IsSuccess = false, Message = "Case not found.", Data = null };

            return new ResponseDto<CaseDto?>
            {
                IsSuccess = true,
                Message = "Case retrieved successfully.",
                Data = MapToDto(caseEntity)
            };
        }
        public async Task<ResponseDto<IEnumerable<CaseDto>>> GetCasesByClientAsync(Guid clientId)
        {
            var cases = await _caseRepository.GetCasesByClientAsync(clientId);
            if (cases == null)
            {
                return new ResponseDto<IEnumerable<CaseDto>>()
                {
                    IsSuccess = false,
                    Message = "Cases not found",
                    Data = null
                };
            }
            return new ResponseDto<IEnumerable<CaseDto>>
            {
                IsSuccess = true,
                Message = "Cases retrieved successfully.",
                Data = cases.Select(MapToDto)
            };
        }
        public async Task<ResponseDto<IEnumerable<CaseDto>>> GetCasesByJudgeAsync(Guid judgeId)
        {
            var cases = await _caseRepository.GetCasesByJudgeAsync(judgeId);
            if (cases == null)
            {
                return new ResponseDto<IEnumerable<CaseDto?>> { IsSuccess = false, Message = "Case not found.", Data = null };
            }
            return new ResponseDto<IEnumerable<CaseDto>>
            {
                IsSuccess = true,
                Message = "Cases retrieved successfully.",
                Data = cases.Select(MapToDto)
            };
        }

        public async Task<ResponseDto<IEnumerable<CaseDto>>> GetCasesByLawyerAsync(Guid lawyerId)
        {
            var cases = await _caseRepository.GetCasesByLawyerAsync(lawyerId);
            if (cases == null)
            {
                return new ResponseDto<IEnumerable<CaseDto>>()
                {
                    IsSuccess = false,
                    Message = "Cases is not found",
                    Data = null
                };
            }
            return new ResponseDto<IEnumerable<CaseDto>>
            {
                IsSuccess = true,
                Message = "Cases retrieved successfully.",
                Data = cases.Select(MapToDto)
            };
        }
        public async Task<ResponseDto<IEnumerable<CaseDto>>> GetClosedCasesAsync()
        {
            var cases = await _caseRepository.GetClosedCasesAsync();
            if (cases == null)
            {
                return new ResponseDto<IEnumerable<CaseDto>>()
                {
                    IsSuccess = false,
                    Message = "Cases not found",
                    Data = null
                };
            }
            return new ResponseDto<IEnumerable<CaseDto>>
            {
                IsSuccess = true,
                Message = "Closed cases retrieved successfully.",
                Data = cases.Select(MapToDto)
            };
        }
        public async Task<ResponseDto<IEnumerable<CaseDto>>> GetPendingCasesAsync()
        {
            var cases = await _caseRepository.GetPendingCasesAsync();
            if (cases == null)
            {
                return new ResponseDto<IEnumerable<CaseDto>>()
                {
                    IsSuccess = false,
                    Message = "No pending cases",
                    Data = null
                };
            }
            return new ResponseDto<IEnumerable<CaseDto>>
            {
                IsSuccess = true,
                Message = "Pending cases retrieved successfully.",
                Data = cases.Select(MapToDto)
            };
        }
        public async Task<ResponseDto<CaseDto>> InitiateCaseFilingAsync(CaseCreateRequestDto newCase, Guid clientId)
        {
            var lawyer = await _lawyerRepository.GetByIdAsync(newCase.LawyerId);
            if (lawyer == null)
            {
                return ResponseDto<CaseDto>.Fail("Lawyer not found");
            }

            var caseEntity = new Case
            {
                CaseNumber = newCase.CaseNumber,
                Title = newCase.Title,
                ClientId = clientId,
                LawyerId = newCase.LawyerId,
                CourtId = newCase.CourtId,
                JudgeId = newCase.JudgeId,
                OffenseType = newCase.OffenseType,
                OffenseLocation = newCase.OffenseLocation,
                Description = newCase.Description,
                Status = CaseStatus.Initiated,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            if (newCase.Parties != null && newCase.Parties.Any())
            {
                caseEntity.CaseParties = newCase.Parties.Select(p => new CaseParty
                {
                    FullName = p.FullName,
                    ContactPhone = p.Contact,
                    ContactEmail = p.Email
                }).ToList();
            }

            await _caseRepository.AddCaseAsync(caseEntity);
            await _unitOfWork.SaveChangesAsync();

            var notification = new NotificationCreateRequestDto
            {
                Title = "New Case Initiated by Client",
                Message = $"A new case has been initiated by client  {clientId}",
                ToId = lawyer.UserId
            };
            await _notificationService.CreateNotificationAsync(notification);

            return new ResponseDto<CaseDto>
            {
                IsSuccess = true,
                Message = "Case initiated successfully.",
                Data = new CaseDto
                {
                    CaseNumber = newCase.CaseNumber,
                    Title = newCase.Title,
                    LawyerId = newCase.LawyerId,
                    CourtId = newCase.CourtId,
                    JudgeId = newCase.JudgeId,
                    OffenseLocation = newCase.OffenseLocation,
                    OffenseType = newCase.OffenseType,
                    Description = newCase.Description
                }
            };
        }
        public async Task NotifyCasePartiesAsync(Guid caseId, string caseNumber)
        {
            var parties = await _casePartyRepository.GetCasePartiesByCaseAsync(caseId);
            foreach (var party in parties)
            {
                await SendMailAsync(party.ContactEmail,party.ContactPhone,caseNumber);
            }

            _logger.LogInformation("Notification sent to {Count} parties for Case ID {CaseId}", parties.Count(), caseId);
        }
        private CaseDto MapToDto(Case c)
        {
            return new CaseDto
            {
                Id = c.Id,
                CaseNumber = c.CaseNumber,
                ClientId = c.ClientId,
                LawyerId = c.LawyerId,
                CourtId = c.CourtId,
                JudgeId = c.JudgeId,
                OffenseType = c.OffenseType,
                OffenseLocation = c.OffenseLocation,
                Description = c.Description,
                Status = c.Status,
                CreatedAt = c.DateCreated,
                UpdatedAt = c.DateModified,
                Parties = c.CaseParties?.Select(p => new CasePartyDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Email = p.ContactEmail,
                    Contact = p.ContactPhone
                }).ToList(),
                Proceedings = c.Proceedings?.Select(p => new CaseProceedingRecordDto
                {
                    Id = p.Id,
                    CourtSessionId = p.CourtSessionId,
                    ProceedingDate = p.DateCreated,
                    Description = p.Description,
                    Outcome = p.Outcome
                }).ToList()
            };
        }
        private async Task SendMailAsync(string email, string name, string caseNumber)
        {
            string title = $"You Have Been Added to Case {caseNumber}";

            string body = $@"
        <html>
        <body style='font-family:Segoe UI, sans-serif; color:#333;'>
            <h2 style='color:#1A73E8;'>JustiSync Case Notification</h2>
            <p>Dear {name},</p>
            <p>
                You have been officially added as a party to <strong>Case {caseNumber}</strong> 
                on the <strong>JustiSync Legal Platform</strong>.
            </p>
            <p>
                You can now view the case details, assigned lawyer, and upcoming proceedings 
                by logging into your JustiSync dashboard.
            </p>
             <p>
                You can view more details about your case by logging in or by creating an account if you dont have one 
                <a href='http://127.0.0.1:5500/GeneralDashboard/Dashboard.html' style='color:#1A73E8; text-decoration:none;'>JustiSync dashboard</a>.
            </p>
            <p>
                Please ensure your contact information is up to date to receive further updates 
                and hearing notifications.
            </p>
            <br/>
            <p>Best regards,<br/><strong>The JustiSync Team</strong></p>
            <hr/>
            <small>This is an automated notification. Please do not reply to this email.</small>
        </body>
        </html>";

            await _emailService.SendEmailAsync(email, title, body);
        }
        private async Task SendMailToCasePartyAsync(string email, string name, string caseNumber, string judgeName, DateTime scheduledDate)
        {
            string title = $"Judge Assigned for Case {caseNumber}";

            string body = $@"
        <html>
        <body style='font-family:Segoe UI, sans-serif; color:#333;'>
            <h2 style='color:#1A73E8;'>JustiSync Case Update</h2>
            <p>Dear {name},</p>
            <p>
                We are pleased to inform you that a judge has been assigned to <strong>Case {caseNumber}</strong>.
            </p>
            <p>
                <strong>Judge Assigned:</strong> {judgeName}<br/>
                <strong>Scheduled Court Date:</strong> {scheduledDate:dddd, MMMM dd, yyyy}
            </p>
            <p>
                Please ensure that you are available on the scheduled date and prepare any necessary documents or information related to your case.
            </p>
            <p>
                You can view more details about your case and its progress by logging into your 
                <a href='http://127.0.0.1:5500/GeneralDashboard/Dashboard.html' style='color:#1A73E8; text-decoration:none;'>JustiSync dashboard</a>.
            </p>
            <br/>
            <p>Best regards,<br/><strong>The JustiSync Team</strong></p>
            <hr/>
            <small>This is an automated message from JustiSync. Please do not reply to this email.</small>
        </body>
        </html>";

            await _emailService.SendEmailAsync(email, title, body);
        }
        private async Task SendMailToJudgeAsync(string email, string firstName, string caseNumber, string courtName, DateTime scheduledDate)
        {
            string title = $"New Case Assignment: {caseNumber}";

            string body = $@"
        <html>
        <body style='font-family:Segoe UI, sans-serif; color:#333;'>
            <h2 style='color:#1A73E8;'>New Case Assignment Notification</h2>
            <p>Dear Hon. Justice {firstName},</p>
            <p>
                You have been assigned to preside over <strong>Case {caseNumber}</strong> 
                at <strong>{courtName}</strong>.
            </p>
            <p>
                <strong>Scheduled Hearing Date:</strong> {scheduledDate:dddd, MMMM dd, yyyy}
            </p>
            <p>
                Kindly review the case details, parties involved, and attached documents in your 
                <a href='https://justisync.app' style='color:#1A73E8; text-decoration:none;'>JustiSync judicial dashboard</a>.
            </p>
            <p>
                Please ensure your availability for the scheduled hearing and confirm any necessary pre-trial actions as applicable.
            </p>
            <br/>
            <p>Best regards,<br/><strong>The JustiSync Registrar Team</strong></p>
            <hr/>
            <small>This is an automated system notification from JustiSync. Please do not reply to this email.</small>
        </body>
        </html>";

            await _emailService.SendEmailAsync(email, title, body);
        }
    }
}