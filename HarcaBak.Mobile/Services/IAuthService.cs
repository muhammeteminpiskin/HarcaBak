using HarcaBak.Mobile.Models;

namespace HarcaBak.Mobile.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);

        Task<bool> RegisterAsync(UserCreateDto userCreateDto);

        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);

        Task<bool> LogoutAsync();
    }
}