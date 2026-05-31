using HarcaBak.Mobile.Helpers;
using HarcaBak.Mobile.Models;
using System.Net.Http.Json;

namespace HarcaBak.Mobile.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiSettings.BaseUrl)
            };
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginDto);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        }

        public async Task<bool> RegisterAsync(UserCreateDto userCreateDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/users", userCreateDto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/change-password", changePasswordDto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LogoutAsync()
        {
            var response = await _httpClient.PostAsync("/api/auth/logout", null);

            if (response.IsSuccessStatusCode)
            {
                SessionManager.Clear();
                return true;
            }

            return false;
        }
    }
}