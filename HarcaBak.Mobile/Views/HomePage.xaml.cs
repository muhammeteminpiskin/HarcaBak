using HarcaBak.Mobile.Helpers;
using HarcaBak.Mobile.Services;

namespace HarcaBak.Mobile.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly ITransactionService _transactionService;
        private readonly IAuthService _authService;

        public HomePage()
        {
            InitializeComponent();

            _transactionService = new TransactionService();
            _authService = new AuthService();

            WelcomeLabel.Text = $"Hoş geldin, {SessionManager.Name}.";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await LoadSummaryAsync();
        }

        private async Task LoadSummaryAsync()
        {
            try
            {
                var summary = await _transactionService.GetSummaryByUserIdAsync(SessionManager.UserId);

                if (summary == null)
                {
                    MessageLabel.Text = "Özet bilgisi alınamadı.";
                    return;
                }

                TotalIncomeLabel.Text = $"Toplam gelir: {summary.TotalIncome:C}";
                TotalExpenseLabel.Text = $"Toplam gider: {summary.TotalExpense:C}";
                BalanceLabel.Text = $"Bakiye: {summary.Balance:C}";

                MessageLabel.Text = "";
            }
            catch
            {
                MessageLabel.Text = "API bağlantısı kurulamadı.";
            }
        }

        private async void OnTransactionsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(TransactionListPage));
        }

        private async void OnAddTransactionClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(TransactionAddPage));
        }
        private async void OnCategoriesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CategoryPage));
        }

        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ChangePasswordPage));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlertAsync(
                "Çıkış Yap",
                "Çıkış yapmak istediğinize emin misiniz?",
                "Evet",
                "Hayır");

            if (!confirm)
            {
                return;
            }

            try
            {
                var isSuccess = await _authService.LogoutAsync();

                if (!isSuccess)
                {
                    await DisplayAlertAsync("Hata", "Çıkış işlemi başarısız oldu.", "Tamam");
                    return;
                }

                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch
            {
                await DisplayAlertAsync("Hata", "API bağlantısı kurulamadı.", "Tamam");
            }
        }

    }
}