using Application.Dto;
using Application.Interfaces.AuthService;
using Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UsersController(ILogger<UsersController> logger, IUserService userService, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
            _userService = userService;
        }

        [SwaggerOperation(Summary = "Register a new user account")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserCreateRequestDto request)
        {
            if (request == null) return BadRequest("Invalid user data.");
            var result = await _userService.RegisterClientAsync(request);
            if (result == null) return BadRequest("User registration failed.");

            return Ok(result);
        }

        [SwaggerOperation(Summary = "Login with email and password to receive authentication token")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Login attempt for user with email: {Email}", request.Email);

            var result = await _userService.LoginAsync(request);
            if (result == null) return Unauthorized("Invalid email or password.");

            return Ok(result);
        }

        [SwaggerOperation(Summary = "Request a password reset link by email")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] UserForgotPasswordRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Processing forgot password request for email: {Email}", request.Email);

            if (request == null) return BadRequest("Invalid request data.");

            var result = await _userService.ForgotPasswordAsync(request);
            if (!result) return BadRequest("Failed to process forgot password request.");

            return Ok("Password reset link sent to your email.");
        }

        [HttpPost("verify-forget-password-code")]
        [SwaggerOperation(Summary = "Verify Forget Password Code",
                   Description = "Verifies the token sent to the user's email for resetting password.")]
        public async Task<ActionResult<ResponseDto<string>>> VerifyForgetPasswordCode([FromBody] VerifyPasswordTokenDto tokenDto)
        {
            if (tokenDto == null)
                return BadRequest(new ResponseDto<string>
                {
                    IsSuccess = false,
                    Message = "Invalid request data",
                    Data = null
                });

            var result = await _userService.VerifyForgetPasswordCodeAsync(tokenDto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [SwaggerOperation(Summary = "Reset password using the reset token")]
        [Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Resetting password for user with email: {Email}", request.Email);

            var result = await _userService.ResetPasswordAsync(request);
            if (!result) return BadRequest("Failed to reset password. Please check your credentials.");

            return Ok("Password reset successfully. You can now log in with your new password.");
        }

        [SwaggerOperation(Summary = "Retrieve a user by unique ID")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (result == null) return NotFound("User not found.");

            return Ok(result);
        }

        [SwaggerOperation(Summary = "Retrieve a user by email address")]
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var result = await _userService.GetUserByEmailAsync(email);
            if (result == null) return NotFound("User not found.");

            return Ok(result);
        }

        [SwaggerOperation(Summary = "Retrieve all users with optional filters and pagination")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers([FromQuery] PageRequest request)
        {
            _logger.LogInformation("Retrieving all users.");
            var result = await _userService.GetAllUsersAsync(request);
            if (result == null) return NotFound("No users found.");

            return Ok(result);
        }

        [SwaggerOperation(Summary = "Update user details by ID")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Updating user with ID: {Id}", id);

            var result = await _userService.UpdateUserAsync(id, request);
            if (result == null) return NotFound("User not found.");

            return Ok(result);
        }
    }
}