using Application.Dto;

namespace Application.IService
{
    public interface IUserService
    {
        Task<ResponseDto<Guid>> RegisterClientAsync(UserCreateRequestDto request);
        Task<ResponseDto<UserDto>> LoginAsync(UserLoginRequestDto request);
        Task<bool> ForgotPasswordAsync(UserForgotPasswordRequestDto request);
        Task<ResponseDto<string>> VerifyForgetPasswordCodeAsync(VerifyPasswordTokenDto tokenDto);
        Task<bool> ResetPasswordAsync(UserResetPasswordRequestDto request);
        Task<UserUpdateRequestDto> UpdateUserAsync(Guid id, UserUpdateRequestDto request);
        Task<ResponseDto<UserDto>> GetUserByIdAsync(Guid id);
        Task<ResponseDto<UserDto>> GetUserByEmailAsync(string email);
        Task<ResponseDto<PageResponse<UserDto>>> GetAllUsersAsync(PageRequest pageRequest);
    }
}