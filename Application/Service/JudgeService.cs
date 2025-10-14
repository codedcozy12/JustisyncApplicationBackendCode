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
        private readonly ICourtRepository _courtRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JudgeService> _logger;
        private readonly ICacheService _cacheService;
        private readonly IUploadService _uploadService;
        private readonly IEmailService _emailService;
        private readonly IAuditLogService _auditLogService;
        public JudgeService( IJudgeRepository judgeRepository,IUnitOfWork unitOfWork, ILogger<JudgeService> logger, ICacheService cacheService, IUploadService uploadService, IEmailService emailService, IUserRepository userRepository, IAuditLogService auditLogService, ICourtRepository courtRepository)
        {
            _judgeRepository = judgeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
            _uploadService = uploadService;
            _emailService = emailService;
            _userRepository = userRepository;
            _auditLogService = auditLogService;
            _courtRepository = courtRepository;
        }
        public async Task<ResponseDto<string>> CheckIfCourtHasJudgeAsync(Guid courtId)
        {
            var court = await _courtRepository.GetByIdAsync(courtId);
            if (court == null)
            {
                _logger.LogWarning("Court with Id: {CourtId} not found", courtId);
                return ResponseDto<string>.Fail($"Court with Id: {courtId} not found");
            }

            
            var existingActiveJudge = await _judgeRepository.GetJudgeByCourtIdAsync(courtId);
            if (existingActiveJudge != null)
            {
                _logger.LogWarning("Court with Id: {CourtId} already has an active judge.", courtId);
                return ResponseDto<string>.Fail("A judge is already assigned to this court. Please remove the current judge before assigning a new one.");
            }

            _logger.LogInformation("No active judge found for Court with Id: {CourtId}", courtId);
            return ResponseDto<string>.Success("No Judge Is Present In This Court.");
        }
        public async Task<ResponseDto<JudgeUpdateRequestDto>> AssignJudgeToCourtAsync(AssignJudgeRequestDto dto)
        {
            var judge = await _judgeRepository.GetByIdAsync(dto.JudgeId);
            if (judge == null || judge.IsDeleted == true)
            {
                _logger.LogWarning("Judge with Id: {JudgeId} not found", dto.JudgeId);
                return ResponseDto<JudgeUpdateRequestDto>.Fail($"Judge with Id: {dto.JudgeId} not found");
            }

            var court = await _courtRepository.GetByIdAsync(dto.CourtId);
            if (court == null)
            {
                _logger.LogWarning("Court with Id: {CourtId} not found", dto.CourtId);
                return ResponseDto<JudgeUpdateRequestDto>.Fail($"Court with Id: {dto.CourtId} not found");
            }


            var existingActiveJudge = await _judgeRepository.GetJudgeByCourtIdAsync(dto.CourtId);
            if (existingActiveJudge != null)
            {
                if(existingActiveJudge.IsDeleted == true)
                {
                    existingActiveJudge.JudgeCourts.FirstOrDefault(a => a.CourtId == dto.CourtId && a.IsActive == true)!.IsActive = false;
                    await _judgeRepository.UpdateAsync(existingActiveJudge);
                    await _unitOfWork.SaveChangesAsync();
                }
                _logger.LogWarning("Court with Id: {CourtId} already has an active judge.", dto.CourtId);
                return ResponseDto<JudgeUpdateRequestDto>.Fail("A judge is already assigned to this court. Please remove the current judge before assigning a new one.");
            }


            bool isJudgeActiveElsewhere = judge.JudgeCourts.Any(jc => jc.IsActive);
            if (isJudgeActiveElsewhere)
            {
                _logger.LogWarning("Judge with Id: {JudgeId} is already assigned to another active court.", dto.JudgeId);
                return ResponseDto<JudgeUpdateRequestDto>.Fail("This judge is already assigned to another active court.");
            }


            var judgeCourt = new JudgeCourt
            {
                JudgeId = dto.JudgeId,
                CourtId = dto.CourtId,
                AssignedDate = DateTime.UtcNow,
                IsActive = true
            };

            judge.JudgeCourts.Add(judgeCourt);
            judge.AppointmentDate = DateTime.UtcNow;

            await _judgeRepository.UpdateAsync(judge);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Judge with Id: {JudgeId} assigned to Court with Id: {CourtId}", dto.JudgeId, dto.CourtId);

            return ResponseDto<JudgeUpdateRequestDto>.Success(new JudgeUpdateRequestDto
            {
                CourtAssignedId = dto.CourtId,
                AppointmentDate = DateTime.UtcNow
            });
        }

        public async Task<ResponseDto<Guid>> CreateJudgeAsync(JudgeCreateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Judge creation request is null");
                return null;
            }
            var validNumber = await _judgeRepository.CheckIfNumberExist(request.PhoneNumber);
            if (validNumber)
            {
                _logger.LogWarning("Judge with Phone Number: {PhoneNumber} already exists", request.PhoneNumber);
                return ResponseDto<Guid>.Fail($"Judge with Phone Number: {request.PhoneNumber} already exists");
            }
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Judge with Email: {Email} already exists", request.Email);
                return  ResponseDto<Guid>.Fail($"Judge with Email: {request.Email} already exists");
            }

            var existingJudge = await _judgeRepository.GetJudgeByCourtIdAsync(request.CourtAssignedId);
            if (existingJudge != null)
            {
                return ResponseDto<Guid>.Fail("A judge is already assigned to this court. Please remove the current judge before assigning a new one.");
            }

            var court = await _courtRepository.GetByIdAsync(request.CourtAssignedId);
            if (court == null)
            {
                _logger.LogWarning("Court with Id: {CourtId} not found", request.CourtAssignedId);
                return ResponseDto<Guid>.Fail($"Court with Id: {request.CourtAssignedId} not found");
            }

            if(court.IsDeleted == true)
            {
                _logger.LogWarning("Court with Id: {CourtId} Is deleted found", request.CourtAssignedId);
                return ResponseDto<Guid>.Fail($"Court with Id: {request.CourtAssignedId} not found");
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
                AppointmentDate = request.AppointmentDate
            };
            judge.JudgeCourts.Add(new JudgeCourt
            {
                AssignedDate = DateTime.UtcNow,
                CourtId = request.CourtAssignedId,
                IsActive = true,
                JudgeId = judge.Id
            });
            await _judgeRepository.AddAsync( judge);
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Judge with Email: {Email} created successfully", request.Email);
            await _auditLogService.LogActionAsync(user.Id, "Create", "Judge", judge.Id, $"Judge {user.Username} created and assigned to court {request.CourtAssignedId}", Role.admin.ToString());
            await SendWelcomeMessageToJudge(user.Email, user.FirstName);
            var data = await SendEmailConfirmationCode(request.Email);
            _logger.LogInformation("Email Sent To Registered Lawyer");
            await _auditLogService.LogActionAsync(user.Id, "SendEmail", "Judge", judge.Id, $"Verification email sent to Judge {user.Username}", Role.admin.ToString());
            return ResponseDto<Guid>.Success(judge.Id,"Registered Successfully");
        }
        public async Task<bool> DeleteJudgeAsync(Guid judgeId)
        {
            var judge = await _judgeRepository.GetByIdAsync(judgeId);
            if (judge.JudgeCourts.FirstOrDefault(a => a.IsActive == true) != null)
            {
                _logger.LogWarning("Cannot delete Judge with Id: {JudgeId} as they are currently assigned to an active court", judgeId);
                return false;
            }
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
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Judge with Id: {JudgeId} deleted successfully", judgeId);
            return true;
        }
        public async Task<ResponseDto<IEnumerable<JudgeDto>>> GetAllJudgesAsync()
        {
            var judges = await _judgeRepository.GetAllAsync();
            var filteredJudges = judges.Where(j => !j.IsDeleted).ToList();
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
                Email = j.User.Email,
                Username = j.User.Username,
                ProfilePictureUrl = j.User.ProfilePictureUrl,
                PhoneNumber = j.User.PhoneNumber,
                MiddleName = j.User.MiddleName,
                LastName = j.User.LastName,
                FirstName = j.User.FirstName,
                CourtAssignedId = j.JudgeCourts.FirstOrDefault(a => a.IsActive == true)?.CourtId,
                IsActive = !j.IsDeleted,
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
            if (judge == null || judge.IsDeleted)
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
                CourtAssignedId = judge.JudgeCourts.FirstOrDefault(a => a.IsActive == true)?.CourtId,
                AppointmentDate = judge.AppointmentDate,
                ProfilePictureUrl = judge.User.ProfilePictureUrl,
                Email = judge.User.Email,
                IsActive = !judge.IsDeleted,
                Username = judge.User.Username
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
            var filteredJudges = judges.Where(j => !j.IsDeleted).ToList();
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
                CourtAssignedId = j.JudgeCourts.FirstOrDefault(a => a.IsActive == true)?.CourtId,
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
           // judge.CourtId = request.CourtAssignedId;
            judge.AppointmentDate = request.AppointmentDate;
            await _judgeRepository.UpdateAsync(judge);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Judge with Id: {JudgeId} updated successfully", judgeId);
            await _auditLogService.LogActionAsync(judge.UserId, "Update", "Judge", judge.Id, $"Judge {judge.User.Username} details updated", Role.admin.ToString());
            return request;
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
        private async Task SendWelcomeMessageToJudge(string email, string name)
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