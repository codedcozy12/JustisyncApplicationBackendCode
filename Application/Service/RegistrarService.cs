using System.Text.Encodings.Web;
using Application.Dto;
using Application.Interface;
using Application.Interfaces.ExternalService;
using Application.Interfaces.Repositories;
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
        public RegistrarService(IRegistrarRepository registrarRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<RegistrarService> logger, IUploadService uploadService, IEmailService emailService, ICacheService cacheService)
        {
            _registrarRepository = registrarRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _logger = logger;
            _emailService = emailService;
            _uploadService = uploadService;
            _cacheService = cacheService;
        }
        public async Task<RegistrarUpdateRequestDto> AssignRegistrarToCourtAsync(Guid registrarId, Guid courtId, DateTime? appointmentDate)
        {
            var registrar = await _registrarRepository.GetByIdAsync(registrarId);
            if (registrar == null)
            {
                _logger.LogWarning("Registrar with Id: {RegistrarId} not found", registrarId);
                return null;
            }
            registrar.CourtId = courtId;
            await _registrarRepository.UpdateAsync(registrar);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Registrar with Id: {RegistrarId} assigned to Court with Id: {CourtId}", registrarId, courtId);
            return new RegistrarUpdateRequestDto
            {
                CourtId = courtId
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
            //var existingRegistrar = await _userRepository.GetByEmailAsync(request.Email);
            //if (existingRegistrar != null)
            //{
            //    _logger.LogWarning("Registrar with Email: {Email} already exists", request.Email);
            //    return ResponseDto<Guid>.Fail("Registrar with Email: {Email} already exists");
            //}
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
            await _registrarRepository.UpdateAsync(registrar);
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
                CourtId = r.CourtId,
                CourtName = r.Court?.Name ?? "Unknown",
                UserId = r.UserId,
                Username = r.User?.Username ?? "Unknown",
                Email = r.User?.Email ?? "Unknown"
            });

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
                CourtName = registrar.Court?.Name ?? "Unknown",
                UserId = registrar.UserId,
                Username = registrar.User?.Username ?? "Unknown",
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
            return request;
        }
        private async Task SendWelcomeMessage(string email, string name)
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