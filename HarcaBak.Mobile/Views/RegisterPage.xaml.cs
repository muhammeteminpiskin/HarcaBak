using HarcaBak.Mobile.Models;
using HarcaBak.Mobile.Services;

namespace HarcaBak.Mobile.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly IAuthService _authService;

        public RegisterPage()
        {
            InitializeComponent();

            _authService = new AuthService();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                MessageLabel.Text = "İsim boş bırakılamaz.";
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailEntry.Text) || !EmailEntry.Text.Contains("@"))
            {
                MessageLabel.Text = "Geçerli bir email adresi giriniz.";
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                MessageLabel.Text = "Şifre boş bırakılamaz.";
                return;
            }

            if (PasswordEntry.Text.Length < 6)
            {
                MessageLabel.Text = "Şifre en az 6 karakter olmalıdır.";
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                MessageLabel.Text = "Şifreler eşleşmiyor.";
                return;
            }

            var userCreateDto = new UserCreateDto
            {
                Name = NameEntry.Text.Trim(),
                Email = EmailEntry.Text.Trim(),
                Password = PasswordEntry.Text
            };

            try
            {
                var isSuccess = await _authService.RegisterAsync(userCreateDto);

                if (!isSuccess)
                {
                    MessageLabel.Text = "Kayıt oluşturulamadı. Email zaten kullanılıyor olabilir.";
                    return;
                }

                await DisplayAlertAsync("Başarılı", "Kayıt oluşturuldu. Giriş yapabilirsiniz.", "Tamam");

                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch
            {
                MessageLabel.Text = "API bağlantısı kurulamadı.";
            }
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}