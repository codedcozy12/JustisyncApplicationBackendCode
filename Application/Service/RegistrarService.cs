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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class RegistrarService : IRegistrarService
    {

        private readonly IRegistrarRepository _registrarRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegistrarService> _logger;
        private readonly IEmailService _emailService;
        private readonly IUploadService _uploadService;
        private readonly ICacheService _cacheService;
        private readonly IAuditLogService _auditLogService;
        public RegistrarService(IRegistrarRepository registrarRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<RegistrarService> logger, IUploadService uploadService, IEmailService emailService, ICacheService cacheService, IAuditLogService auditLogService)
        {
            _registrarRepository = registrarRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _logger = logger;
            _emailService = emailService;
            _uploadService = uploadService;
            _cacheService = cacheService;
            _auditLogService = auditLogService;
        }
        public async Task<RegistrarUpdateRequestDto> AssignRegistrarToCourtAsync(AssignRegistrarRequestDto dto)
        {
            var registrar = await _registrarRepository.GetByIdAsync(dto.RegistrarId);
            if (registrar == null)
            {
                _logger.LogWarning("Registrar with Id: {RegistrarId} not found", dto.RegistrarId);
                return null;
            }
            registrar.CourtId = dto.CourtId;
            await _registrarRepository.UpdateAsync(registrar);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Registrar with Id: {RegistrarId} assigned to Court with Id: {CourtId}", dto.RegistrarId, dto.CourtId);
            return new RegistrarUpdateRequestDto
            {
                CourtId = dto.CourtId,
            };
        }
        public async Task<ResponseDto<Guid>> CreateRegistrarAsync(RegistrarCreateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Registrar creation request is null");
                return null;
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
                Role = Role.registrar,
                Accountstatus = Accountstatus.Active
            };
            if (request.ProfilePictureUrl != null)
            {
                var profilePicturePath = _uploadService.UploadProfile(request.ProfilePictureUrl);
                user.ProfilePictureUrl = profilePicturePath;
            }
            var registrar = new Registrar
            {
                CourtId = request.CourtId,
                UserId = user.Id
            };
            await _userRepository.AddAsync(user);
            await _registrarRepository.AddAsync(registrar);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Registrar with Email: {Email} created successfully", request.Email);
            await _auditLogService.LogActionAsync(user.Id, "Create", "Registrar", registrar.Id, $"Registrar {user.Email} created and assigned to CourtId {registrar.CourtId}", Role.admin.ToString());
            await SendWelcomeMessageToRegistrar(user.Email, user.FirstName);
            var data = await SendEmailConfirmationCode(request.Email);
            _logger.LogInformation("Email Sent To Registered Registrar");
            await _auditLogService.LogActionAsync(user.Id, "SendEmail", "Registrar", registrar.Id, $"Email confirmation code sent to {user.Email}", Role.admin.ToString());
            return ResponseDto<Guid>.Success(user.Id, "Registered Successfully");
        }

        public async Task<bool> DeleteRegistrarAsync(Guid registrarId)
        {
            var registrar = await _registrarRepository.GetByIdAsync(registrarId);
            if (registrar == null)
            {
                _logger.LogWarning("Registrar with Id: {RegistrarId} not found", registrarId);
                return false;
            }
            var user = await _userRepository.GetByIdAsync(registrar.UserId);
            if (user == null)
            {
                _logger.LogWarning("User associated with Registrar Id: {RegistrarId} not found", registrarId);
                return false;
            }
            registrar.IsDeleted = true;
            user.IsDeleted = true;
            await _registrarRepository.UpdateAsync(registrar);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Registrar with Id: {RegistrarId} deleted successfully", registrarId);
            return true;
        }
        public async Task<ResponseDto<IEnumerable<RegistrarDto>>> GetAllRegistrarsAsync()
        {
            var registrars = await _registrarRepository.GetAllAsync();

            if (registrars == null || !registrars.Any())
            {
                _logger.LogWarning("No registrars found");
                return ResponseDto<IEnumerable<RegistrarDto>>.Fail("No registrars found");
            }

            var registrarDtos = registrars.Select(r => new RegistrarDto
            {
                Id = r.Id,
                FirstName = r.User.FirstName,
                LastName = r.User.LastName,
                MiddleName = r.User.MiddleName,
                PhoneNumber = r.User.PhoneNumber,
                ProfilePictureUrl = r.User.ProfilePictureUrl,
                Email = r.User.Email,
                CourtId = r.CourtId,
                CourtName = r.Court.Name,
                Username = r.User.Username,
                IsActive = !r.IsDeleted,
                UserId = r.UserId
            }).ToList();

            _logger.LogInformation("Retrieved all registrars successfully");

            return ResponseDto<IEnumerable<RegistrarDto>>.Success(registrarDtos);
        }
        public async Task<ResponseDto<RegistrarDto>> GetRegistrarByIdAsync(Guid registrarId)
        {
            var registrar = await _registrarRepository.GetByIdAsync(registrarId);
            if (registrar == null)
            {
                _logger.LogWarning("Registrar with Id: {RegistrarId} not found", registrarId);
                return ResponseDto<RegistrarDto>.Fail("Registrar not found");
            }
            var registrarDto = new RegistrarDto
            {
                Id = registrar.Id,
                CourtId = registrar.CourtId,
                FirstName = registrar.User.FirstName,
                LastName = registrar.User.LastName,
                MiddleName = registrar.User.MiddleName,
                PhoneNumber = registrar.User.PhoneNumber,
                ProfilePictureUrl = registrar.User.ProfilePictureUrl,
                CourtName = registrar.Court?.Name ?? "Unknown",
                UserId = registrar.UserId,
                Username = registrar.User?.Username ?? "Unknown",
                IsActive = !registrar.IsDeleted,
                Email = registrar.User?.Email ?? "Unknown"
            };
            _logger.LogInformation("Registrar with Id: {RegistrarId} retrieved successfully", registrarId);
            return ResponseDto<RegistrarDto>.Success(registrarDto);
        }
        public async Task<ResponseDto<IEnumerable<RegistrarDto>>> GetRegistrarsByCourtAsync(Guid courtId)
        {
            var registrars = await _registrarRepository.GetByCourtIdAsync(courtId);

            if (registrars == null || !registrars.Any())
            {
                _logger.LogWarning("No registrars found for Court with Id: {CourtId}", courtId);
                return ResponseDto<IEnumerable<RegistrarDto>>.Fail("No registrars found for the specified court");
            }

            var registrarDtos = registrars.Select(r => new RegistrarDto
            {
                Id = r.Id,
                CourtId = r.CourtId,
                CourtName = r.Court?.Name ?? "Unknown",
                UserId = r.UserId,
                Username = r.User?.Username ?? "Unknown",
                Email = r.User?.Email ?? "Unknown"
            });

            _logger.LogInformation("Retrieved registrars for Court with Id: {CourtId} successfully", courtId);

            return ResponseDto<IEnumerable<RegistrarDto>>.Success(registrarDtos);
        }
        public async Task<RegistrarUpdateRequestDto> UpdateRegistrarAsync(Guid registrarId, RegistrarUpdateRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Registrar update request is null");
                return null;
            }
            var registrar = await _registrarRepository.GetByIdAsync(registrarId);
            if (registrar == null)
            {
                _logger.LogWarning("Registrar with Id: {RegistrarId} not found", registrarId);
                return null;
            }
            registrar.CourtId = request.CourtId;
            await _registrarRepository.UpdateAsync(registrar);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Registrar with Id: {RegistrarId} updated successfully", registrarId);
            await _auditLogService.LogActionAsync(registrar.UserId, "Update", "Registrar", registrar.Id, $"Registrar {registrar.User.Email} updated. Assigned to CourtId {registrar.CourtId}", Role.admin.ToString());
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
        private async Task SendWelcomeMessageToRegistrar(string email, string name)
    {
        var subject = "Welcome to JustiSyncApplication — Registrar Account Ready";

        var safeName = HtmlEncoder.Default.Encode(string.IsNullOrWhiteSpace(name) ? "Registrar" : name);

        var html = $@"
<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width,initial-scale=1"">
  <title>Welcome to JustiSyncApplication</title>
  <style>
    body {{ margin:0; padding:0; background:#f5f7fb; font-family:Segoe UI, Roboto, Arial, sans-serif; color:#0f172a; }}
    .container {{ max-width:640px; margin:24px auto; padding:16px; }}
    .card {{ background:#fff; border-radius:12px; padding:28px; box-shadow:0 6px 20px rgba(15,23,42,0.06); }}
    .brand {{ font-weight:700; color:#0b5fff; font-size:18px; }}
    h1 {{ font-size:22px; margin:12px 0; }}
    p {{ color:#374151; line-height:1.6; }}
    .btn {{ display:inline-block; padding:10px 16px; background:#0b5fff; color:#fff !important; text-decoration:none; border-radius:8px; font-weight:600; }}
    .muted {{ color:#6b7280; font-size:13px; }}
    .divider {{ height:1px; background:#e6eefc; margin:20px 0; border-radius:2px; }}
    ul {{ padding-left:18px; }}
    .card-small {{ background:#f8fbff; border:1px solid #e6f0ff; padding:12px; border-radius:8px; margin-top:12px; }}
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""card"">
      <div class=""brand"">JustiSyncApplication</div>
      <h1>Welcome, {safeName} 👋</h1>

      <p>Thank you for joining <strong>JustiSyncApplication</strong> as a <strong>Registrar</strong>. Your role is vital — you help keep our lawyer verification process and court assignments running smoothly.</p>

      <div class=""card-small"">
        <strong>Quick actions to get started</strong>
        <ul>
          <li>Review pending lawyer certificates and verification requests.</li>
          <li>Assign verified lawyers and judges to courts.</li>
          <li>Manage case administrative tasks and notifications.</li>
        </ul>
      </div>

      <div class=""divider""></div>

      <p>If you need assistance at any time, reply to this email or contact our support team. We’ll also send notifications inside the app when actions require your attention.</p>

      <p>Thanks for helping keep JustiSync secure and trusted.<br/>— The JustiSyncApplication Team</p>

      <p class=""muted"" style=""margin-top:12px;"">© {DateTime.UtcNow:yyyy} JustiSyncApplication. All rights reserved.</p>
    </div>
  </div>
</body>
</html>";

        await _emailService.SendEmailAsync(email, subject, html);
        }
    }
}