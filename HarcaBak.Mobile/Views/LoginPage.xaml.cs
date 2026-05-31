using HarcaBak.Mobile.Helpers;
using HarcaBak.Mobile.Models;
using HarcaBak.Mobile.Services;

namespace HarcaBak.Mobile.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly IAuthService _authService;

        public LoginPage()
        {
            InitializeComponent();

            _authService = new AuthService();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailEntry.Text))
            {
                MessageLabel.Text = "Email boş bırakılamaz.";
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                MessageLabel.Text = "Şifre boş bırakılamaz.";
                return;
            }

            var loginDto = new LoginDto
            {
                Email = EmailEntry.Text.Trim(),
                Password = PasswordEntry.Text
            };

            try
            {
                var loginResult = await _authService.LoginAsync(loginDto);

                if (loginResult == null)
                {
                    MessageLabel.Text = "Email veya şifre hatalı.";
                    return;
                }

                SessionManager.SetUser(
                    loginResult.UserId,
                    loginResult.Name,
                    loginResult.Email);

                MessageLabel.Text = $"Hoş geldin, {loginResult.Name}.";

                await Shell.Current.GoToAsync(nameof(HomePage));

                
            }
            catch
            {
                MessageLabel.Text = "API bağlantısı kurulamadı. Backend çalışıyor mu kontrol et.";
            }

        }
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}