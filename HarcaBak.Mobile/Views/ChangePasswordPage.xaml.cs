using HarcaBak.Mobile.Helpers;
using HarcaBak.Mobile.Models;
using HarcaBak.Mobile.Services;

namespace HarcaBak.Mobile.Views
{
    public partial class ChangePasswordPage : ContentPage
    {
        private readonly IAuthService _authService;

        public ChangePasswordPage()
        {
            InitializeComponent();

            _authService = new AuthService();
        }

        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OldPasswordEntry.Text))
            {
                MessageLabel.Text = "Eski şifre boş bırakılamaz.";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPasswordEntry.Text))
            {
                MessageLabel.Text = "Yeni şifre boş bırakılamaz.";
                return;
            }

            if (NewPasswordEntry.Text.Length < 6)
            {
                MessageLabel.Text = "Yeni şifre en az 6 karakter olmalıdır.";
                return;
            }

            if (NewPasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                MessageLabel.Text = "Yeni şifreler eşleşmiyor.";
                return;
            }

            var changePasswordDto = new ChangePasswordDto
            {
                UserId = SessionManager.UserId,
                OldPassword = OldPasswordEntry.Text,
                NewPassword = NewPasswordEntry.Text
            };

            try
            {
                var isSuccess = await _authService.ChangePasswordAsync(changePasswordDto);

                if (!isSuccess)
                {
                    MessageLabel.Text = "Eski şifre hatalı veya şifre değiştirilemedi.";
                    return;
                }

                MessageLabel.Text = "Şifre başarıyla değiştirildi.";

                OldPasswordEntry.Text = string.Empty;
                NewPasswordEntry.Text = string.Empty;
                ConfirmPasswordEntry.Text = string.Empty;
            }
            catch
            {
                MessageLabel.Text = "API bağlantısı kurulamadı.";
            }
        }
    }
}