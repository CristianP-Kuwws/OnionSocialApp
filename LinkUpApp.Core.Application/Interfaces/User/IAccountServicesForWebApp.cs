using LinkUpApp.Core.Application.Dtos.User;
using LinkUpApp.Core.Application.Dtos.User.Password;

namespace LinkUpApp.Core.Application.Interfaces.User
{
    public interface IAccountServicesForWebApp
    {
        Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto);
        Task<string> ConfirmAccountAsync(string userId, string token);
        Task<UserResponseDto> DeleteAsync(string id);
        Task<EditResponseDto> EditUser(SaveUserDto saveDto, string origin, bool? isCreated = false);
        Task<UserResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<List<UserDto>> GetAllUser(bool? isActive = true);
        Task<UserDto?> GetUserByEmail(string email);
        Task<UserDto?> GetUserByUsername(string userName);
        Task<UserDto?> GetUserById(string id);
        Task<RegisterResponseDto> RegisterUser(SaveUserDto saveDto, string origin);
        Task<UserResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task SingOutAsync();
    }
}