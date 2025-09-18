using System.Text.Encodings.Web;
using Application.Dto;
using Application.Interface;
using Application.Interfaces.ExternalService;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class JudgeService : IJudgeService
    {
        private readonly IJudgeRepository _judgeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JudgeService> _logger;
        private readonly ICacheService _cacheService;
        private readonly IUploadService _uploadService;
        private readonly IEmailService _emailService;
        public JudgeService( IJudgeRepository judgeRepository, IUnitOfWork unitOfWork, ILogger<JudgeService> logger, ICacheService cacheService, IUploadService uploadService, IEmailService emailService,IUserRepository userRepository)
        {
            _judgeRepository = judgeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
            _uploadService = uploadService;
            _emailService = emailService;
            _userRepository = userRepository;
        }
        public async Task<JudgeUpdateRequestDto> AssignJudgeToCourtAsync(Guid judgeId, Guid courtId, DateTime? appointmentDate)
        {
            var judge = await _judgeRepository.GetByIdAsync(judgeId);
            if (judge == null)
            {
                _logger.LogWarning("Judge with Id: {JudgeId} not found", judgeId);
                return null;
            }
            judge.CourtId = courtId;
            judge.AppointmentDate = appointmentDate;
            await _judgeRepository.UpdateAsync(judge);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Judge with Id: {JudgeId} assigned to Court with Id: {CourtId}", judgeId, courtId);
            return new JudgeUpdateRequestDto
            {
                CourtAssignedId = courtId,
                AppointmentDate = appointmentDate
            };
        }
        public async Task<ResponseDto<JudgeCreateRequestDto>> CreateJudgeAsync(JudgeCreateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Judge creation request is null");
                return null;
            }
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Judge with Email: {Email} already exists", request.Email);
                return  ResponseDto<JudgeCreateRequestDto>.Fail($"Judge with Email: {request.Email} already exists");
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
                Role = Role.judge,
                Accountstatus = Accountstatus.Active
            };
            if (request.ProfilePictureUrl != null)
            {
                var profilePicturePath = _uploadService.UploadProfile(request.ProfilePictureUrl);
                user.ProfilePictureUrl = profilePicturePath;
            }
            var judge = new Judge
            {
                UserId = user.Id,
                CourtId = request.CourtAssignedId,
                AppointmentDate = request.AppointmentDate
            };
            await _judgeRepository.AddAsync( judge);
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Judge with Email: {Email} created successfully", request.Email);
            return ResponseDto<JudgeCreateRequestDto>.Success(request,"Registered Successfully");
        }
        public async Task<bool> DeleteJudgeAsync(Guid judgeId)
        {
            var judge = await _judgeRepository.GetByIdAsync(judgeId);
            if (judge == null)
            {
                _logger.LogWarning("Judge with Id: {JudgeId} not found", judgeId);
                return false;
            }
            var user = await _userRepository.GetByIdAsync(judge.UserId); 
            if(user == null)
            {
                _logger.LogWarning("User associated with Judge Id: {JudgeId} not found", judgeId);
                return false;
            }
            judge.IsDeleted = true;
            user.IsDeleted = true;
            await _judgeRepository.UpdateAsync(judge);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Judge with Id: {JudgeId} deleted successfully", judgeId);
            return true;
        }
        public async Task<ResponseDto<IEnumerable<JudgeDto>>> GetAllJudgesAsync()
        {
            var judges = await _judgeRepository.GetAllAsync();
            if (judges == null || !judges.Any())
            {
                _logger.LogInformation("No judges found");
                return new ResponseDto<IEnumerable<JudgeDto>>
                {
                    Data = Enumerable.Empty<JudgeDto>(),
                    IsSuccess = false,
                    Message = "No judges found"
                };
            }

            var judgeDtos = judges.Select(j => new JudgeDto
            {
                Id = j.Id,
                UserId = j.UserId,
                PhoneNumber = j.User.PhoneNumber,
                MiddleName = j.User.MiddleName,
                LastName = j.User.LastName,
                FirstName = j.User.FirstName,
                CourtAssignedId = j.CourtId,
                AppointmentDate = j.AppointmentDate
            }).ToList();

            _logger.LogInformation("Retrieved {Count} judges successfully", judgeDtos.Count);

            return new ResponseDto<IEnumerable<JudgeDto>>
            {
                Data = judgeDtos,
                IsSuccess = true,
                Message = "Judges retrieved successfully"
            };
        }

        public async Task<ResponseDto<JudgeDto>> GetJudgeByIdAsync(Guid judgeId)
        {
            var judge = await _judgeRepository.GetByIdAsync(judgeId);
            if (judge == null)
            {
                _logger.LogWarning("Judge with Id: {JudgeId} not found", judgeId);
                return new ResponseDto<JudgeDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Judge not found"
                };
            }
            var judgeDto = new JudgeDto
            {
                Id = judge.Id,
                UserId = judge.UserId,
                PhoneNumber = judge.User.PhoneNumber,
                MiddleName = judge.User.MiddleName,
                LastName = judge.User.LastName,
                FirstName = judge.User.FirstName,
                CourtAssignedId = judge.CourtId,
                AppointmentDate = judge.AppointmentDate
            };
            _logger.LogInformation("Retrieved Judge with Id: {JudgeId} successfully", judgeId);
            return new ResponseDto<JudgeDto>
            {
                Data = judgeDto,
                IsSuccess = true,
                Message = "Judge retrieved successfully"
            };
        }
        public async Task<ResponseDto<IEnumerable<JudgeDto>>> GetJudgesByCourtAsync(Guid courtId)
        {
            var judges = await _judgeRepository.GetByCourtIdAsync(courtId);
            if (judges == null || !judges.Any())
            {
                _logger.LogInformation("No judges found for Court with Id: {CourtId}", courtId);
                return new ResponseDto<IEnumerable<JudgeDto>>
                {
                    Data = Enumerable.Empty<JudgeDto>(),
                    IsSuccess = false,
                    Message = "No judges found for the specified court"
                };
            }
            var judgeDtos = judges.Select(j => new JudgeDto
            {
                Id = j.Id,
                UserId = j.UserId,
                PhoneNumber = j.User.PhoneNumber,
                MiddleName = j.User.MiddleName,
                LastName = j.User.LastName,
                FirstName = j.User.FirstName,
                CourtAssignedId = j.CourtId,
                AppointmentDate = j.AppointmentDate
            }).ToList();
            _logger.LogInformation("Retrieved {Count} judges for Court with Id: {CourtId} successfully", judgeDtos.Count(), courtId);
            return new ResponseDto<IEnumerable<JudgeDto>>
            {
                Data = judgeDtos,
                IsSuccess = true,
                Message = "Judges retrieved successfully"
            };
        }
        public async Task<JudgeUpdateRequestDto> UpdateJudgeAsync(Guid judgeId, JudgeUpdateRequestDto request)
        {
            var judge = await _judgeRepository.GetByIdAsync(judgeId);
            if (judge == null)
            {
                _logger.LogWarning("Judge with Id: {JudgeId} not found", judgeId);
                return null;
            }
            judge.CourtId = request.CourtAssignedId;
            judge.AppointmentDate = request.AppointmentDate;
            await _judgeRepository.UpdateAsync(judge);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Judge with Id: {JudgeId} updated successfully", judgeId);
            return request;
        }
        private async Task SendWelcomeMessage(string email, string name)
    {
        var subject = "Welcome to JustiSyncApplication — Judge Account Activated";

        var safeName = HtmlEncoder.Default.Encode(string.IsNullOrWhiteSpace(name) ? "Judge" : name);

        var dashboardUrl = "https://app.justisync.example/judge/dashboard";

        var supportEmail = "support@justisync.example";

        var html = $@"
<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width,initial-scale=1"">
  <title>Welcome to JustiSyncApplication</title>
  <style>
    body {{ margin:0; padding:0; background:#f4f6fa; font-family:Segoe UI, Roboto, Arial, sans-serif; color:#111827; }}
    .container {{ max-width:640px; margin:24px auto; padding:16px; }}
    .card {{ background:#fff; border-radius:12px; padding:28px; box-shadow:0 6px 20px rgba(0,0,0,0.06); }}
    .brand {{ font-weight:700; color:#0b5fff; font-size:18px; }}
    h1 {{ font-size:22px; margin:12px 0; }}
    p {{ color:#374151; line-height:1.6; }}
    .btn {{ display:inline-block; padding:10px 16px; background:#0b5fff; color:#fff !important; text-decoration:none; border-radius:8px; font-weight:600; }}
    .muted {{ color:#6b7280; font-size:13px; }}
    .divider {{ height:1px; background:#e6eaf5; margin:20px 0; border-radius:2px; }}
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""card"">
      <div class=""brand"">JustiSyncApplication</div>
      <h1>Welcome, {safeName} ⚖️</h1>

      <p>We’re honored to have you join <strong>JustiSyncApplication</strong> as a <strong>Judge</strong>. 
      Your role is essential in ensuring fair, transparent, and efficient case handling across our platform.</p>

      <p>With your Judge account, you can:</p>
      <ul>
        <li>Preside over assigned cases with secure document access.</li>
        <li>Review case filings and submissions from lawyers and clients.</li>
        <li>Manage court sessions and issue rulings directly in the platform.</li>
        <li>Collaborate with registrars for smooth scheduling and case tracking.</li>
      </ul>

      <p style=""margin-top:18px;"">
        <a class=""btn"" href=""{dashboardUrl}"" target=""_blank"" rel=""noopener"">Access Your Judge Dashboard</a>
      </p>

      <div class=""divider""></div>

      <p>If you need any assistance, our team is here for you at 
        <a href=""mailto:{supportEmail}"">{supportEmail}</a>.</p>

      <p>Thank you for bringing your expertise and fairness to the JustiSync community.<br/>
      — The JustiSyncApplication Team</p>

      <p class=""muted"" style=""margin-top:12px;"">© {DateTime.UtcNow:yyyy} JustiSyncApplication. All rights reserved.</p>
    </div>
  </div>
</body>
</html>";
        await _emailService.SendEmailAsync(email, subject, html);
        }
    }
}