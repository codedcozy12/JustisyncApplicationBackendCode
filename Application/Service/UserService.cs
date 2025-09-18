using System.Net;
using Application.Dto;
using Application.Interface;
using Application.Interfaces.ExternalService;
using Application.Interfaces.UnitOfWork;
using Application.IService;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private readonly IUploadService _uploadService;
        private readonly ICacheService _cacheService;
        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<UserService> logger, IUploadService uploadService, IEmailService emailService, ICacheService cacheService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _uploadService = uploadService;
            _emailService = emailService;
            _cacheService = cacheService;
        }
        public async Task<bool> ForgotPasswordAsync(UserForgotPasswordRequestDto request)
        {
            _logger.LogInformation("Password reset request for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Password reset failed. Email {Email} not found.", request.Email);
                return false;
            }
             
            string code = new Random().Next(100000,999999).ToString();
            string cacheKey = $"ForgotPasswordToken:{request.Email}";

            await _cacheService.AddAsync(cacheKey, code, TimeSpan.FromMinutes(5));

            await SendForgotPasswordEmail(request.Email, code);

            _logger.LogInformation("Password reset token generated for user Id: {UserId}", user.Id);

            return true;
        }
        public async Task<ResponseDto<string>> VerifyForgetPasswordCodeAsync(VerifyPasswordTokenDto tokenDto)
        {
            var user = await _userRepository.GetByEmailAsync(tokenDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Password reset failed. Email {Email} not found.", tokenDto.Email);
                return ResponseDto<string>.Fail("User Not Found");
            }

            _logger.LogInformation("Verifying password reset token for email: {Email}", tokenDto.Email);
            var cacheKey = $"ForgotPasswordToken:{tokenDto.Email}";
            var cachedCode = await _cacheService.GetAsync<string>(cacheKey);

            if (cachedCode == null || cachedCode != tokenDto.Token)
            {
                _logger.LogWarning("Invalid or expired password reset token for email: {Email}", tokenDto.Token);
                return ResponseDto<string>.Fail("Invalid or expired token.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tokenDto.Password);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Password reset token verified successfully for email: {Email}", tokenDto.Token);
            return ResponseDto<string>.Success("Token verified successfully.");
        }

        public async Task<ResponseDto<UserDto>> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found.", email);
                return ResponseDto<UserDto>.Fail("User not found.");
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                MiddleName = user.MiddleName,
                PhoneNumber = user.PhoneNumber,
                Username = user.Username,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
            _logger.LogInformation("User with email {Email} retrieved successfully.", email);
            return ResponseDto<UserDto>.Success(userDto);
        }
        public async Task<ResponseDto<PageResponse<UserDto>>> GetAllUsersAsync(PageRequest pageRequest)
        {
            var response = await _userRepository.GetAllAsync(pageRequest);
            var userDtos = response.Items.Select(user => new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                MiddleName = user.MiddleName,
                PhoneNumber = user.PhoneNumber,
                Username = user.Username,
                ProfilePictureUrl = user.ProfilePictureUrl
            }).ToList();
            var pageResponse = new PageResponse<UserDto>
            {
                Items = userDtos,
                TotalCount = response.TotalCount,
                PageNumber = response.PageNumber,
                PageSize = response.PageSize
            };
            return ResponseDto<PageResponse<UserDto>>.Success(pageResponse);
        }
        public async Task<ResponseDto<UserDto>> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving user by Id: {UserId}", id);

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with Id {UserId} not found.", id);
                return ResponseDto<UserDto>.Fail("User not found.");
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                MiddleName = user.MiddleName,
                PhoneNumber = user.PhoneNumber,
                Username = user.Username,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
            _logger.LogInformation("User with Id {UserId} retrieved successfully.", id);
            return ResponseDto<UserDto>.Success(userDto);
        }
        public async Task<ResponseDto<UserDto>> LoginAsync(UserLoginRequestDto request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("Login failed. Email {Email} not found.", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password,user.PasswordHash))
            {
                _logger.LogWarning("Login failed for {Email}. Incorrect password.", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            _logger.LogInformation("Login successful for user Id: {UserId}", user.Id);

            return ResponseDto<UserDto>.Success(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Role = user.Role,
                Username = user.Username,
                Accountstatus = user.Accountstatus
            },"Login Successful");
        }
        public async Task<ResponseDto<Guid>> RegisterClientAsync(UserCreateRequestDto request)
        {
            _logger.LogInformation("Attempting to register a new client with email: {Email}", request.Email);
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed. Email {Email} already exists.", request.Email);
                throw new InvalidOperationException("Email already exists.");
            }

            string? pictureUrl = null;
            if (request.ProfilePictureUrl != null)
            {
                pictureUrl = _uploadService.UploadProfile(request.ProfilePictureUrl);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = Role.client,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                MiddleName = request.MiddleName,
                PhoneNumber = request.PhoneNumber,
                Username = request.Username,
                CreatedBy = request.Username,
                ProfilePictureUrl = pictureUrl,
            };
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Successfully registered client with Id: {UserId}", user.Id);
            await SendWelcomeMessage(request.Email, request.FirstName);
            _logger.LogInformation("Email Sent To Registered Client");
            return ResponseDto<Guid>.Success(user.Id,"User Registration Was Successful");
        }
        public async Task<bool> ResetPasswordAsync(UserResetPasswordRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Password reset failed. Email {Email} not found.", request.Email);
                return false;
            }
            _logger.LogInformation("Resetting password for user Id: {UserId}", user.Id);
            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                _logger.LogWarning("Password reset failed for user Id: {UserId}. Incorrect old password.", user.Id);
                return false;
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Password reset successful for user Id: {UserId}", user.Id);
            return true;
        }
        public async Task<UserUpdateRequestDto> UpdateUserAsync(Guid id, UserUpdateRequestDto request)
        {
            _logger.LogInformation("Updating user with Id: {UserId}", id);
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Update failed. User with Id {UserId} not found.", id);
                throw new InvalidOperationException("User not found.");
            }
            if (request.Username != null)
                user.Username = request.Username;
            if (request.Email != null)
                user.Email = request.Email;
            if (request.FirstName != null)
                user.FirstName = request.FirstName;
            if (request.LastName != null)
                user.LastName = request.LastName;
            if (request.MiddleName != null)
                user.MiddleName = request.MiddleName;
            if (request.PhoneNumber != null)
                user.PhoneNumber = request.PhoneNumber;
            if (request.ProfilePictureUrl != null)
            {
                user.ProfilePictureUrl = _uploadService.UploadProfile(request.ProfilePictureUrl);
            }
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("User with Id {UserId} updated successfully.", id);
            return request;
        }
    private async Task SendWelcomeMessage(string email, string name)
    {
        var safeName = WebUtility.HtmlEncode(name);

        var subject = "Welcome to JustiSyncApplication 🎉";

        var html = $@"
<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width,initial-scale=1"">
  <title>Welcome to JustiSyncApplication</title>
  <style>
    body {{ margin:0; padding:0; background:#f6f7fb; font-family:Segoe UI, Roboto, Helvetica, Arial, sans-serif; color:#12141a; }}
    .container {{ max-width:640px; margin:0 auto; padding:24px 16px; }}
    .card {{ background:#ffffff; border-radius:16px; padding:32px; box-shadow:0 8px 24px rgba(18,20,26,0.06); }}
    .brand {{ font-size:18px; font-weight:600; color:#1a1f36; }}
    .h1 {{ font-size:24px; margin:16px 0 8px; color:#1a1f36; }}
    .lead {{ font-size:16px; line-height:1.6; color:#3c4053; }}
    .btn {{ display:inline-block; margin:24px 0 8px; padding:12px 20px; background:#2f6fed; color:#fff !important; text-decoration:none; border-radius:10px; font-weight:600; }}
    .muted {{ font-size:12px; color:#72768a; }}
    .divider {{ height:1px; background:#eceef5; margin:28px 0; }}
    .list {{ padding-left:18px; }}
    a {{ color:#2f6fed; }}
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""card"">
      <div class=""brand"">JustiSyncApplication</div>

      <h1 class=""h1"">Welcome, {safeName}! 👋</h1>
      <p class=""lead"">
        Thank you for joining <b>JustiSyncApplication</b>. We’re excited to support you in managing your legal needs with ease and confidence.
      </p>

      <p class=""lead"">As a client, you can:</p>
      <ul class=""lead list"">
        <li>File and track your cases seamlessly</li>
        <li>Connect with verified lawyers</li>
        <li>Receive timely updates on case progress</li>
        <li>Access important documents anytime, anywhere</li>
      </ul>

      <a class=""btn"" href=""https://app.justisync.example/login"" target=""_blank"" rel=""noopener"">Get Started</a>
      <div class=""muted"">
        If the button doesn’t work, paste this link in your browser:<br>
        <a href=""https://app.justisync.example/login"">https://app.justisync.example/login</a>
      </div>

      <div class=""divider""></div>

      <p class=""muted"">
        Need help? Reply to this email or visit our Help Center.<br>
        © {DateTime.UtcNow:yyyy} JustiSyncApplication. All rights reserved.
      </p>
    </div>
  </div>
</body>
</html>";
        await _emailService.SendEmailAsync(email, subject, html);
    }
        private async Task SendForgotPasswordEmail(string email, string code)
        {
            var subject = "JustiSyncApplication - Password Reset Code";

            var message = $@"
    <html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }}
            .container {{
                max-width: 600px;
                margin: 40px auto;
                background: #ffffff;
                padding: 20px;
                border-radius: 10px;
                box-shadow: 0 2px 6px rgba(0,0,0,0.15);
            }}
            h2 {{
                color: #2c3e50;
            }}
            .code {{
                font-size: 22px;
                font-weight: bold;
                color: #ffffff;
                background-color: #3498db;
                padding: 10px 20px;
                border-radius: 8px;
                display: inline-block;
                margin: 15px 0;
                letter-spacing: 2px;
            }}
            p {{
                color: #555555;
                line-height: 1.6;
            }}
            .footer {{
                margin-top: 25px;
                font-size: 12px;
                color: #999999;
                text-align: center;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>Password Reset Request</h2>
            <p>Hello,</p>
            <p>We received a request to reset your password for <strong>JustiSyncApplication</strong>. 
            Use the following code to complete your reset process:</p>
            <div class='code'>{code}</div>
            <p>If you didn’t request a password reset, you can safely ignore this email. 
            Your account remains secure.</p>
            <div class='footer'>
                © {DateTime.UtcNow.Year} JustiSyncApplication. All rights reserved.
            </div>
        </div>
    </body>
    </html>";
            await _emailService.SendEmailAsync(email, subject, message);
        }
    }
}