using HarcaBak.Mobile.Helpers;
using HarcaBak.Mobile.Models;
using HarcaBak.Mobile.Services;
using System.Collections.ObjectModel;

namespace HarcaBak.Mobile.Views
{
    public partial class TransactionListPage : ContentPage
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;

        private readonly ObservableCollection<TransactionListDto> _transactions = new();

        private List<CategoryListDto> _categories = new();

        public TransactionListPage()
        {
            InitializeComponent();

            _transactionService = new TransactionService();
            _categoryService = new CategoryService();

            TransactionsCollectionView.ItemsSource = _transactions;

            StartDatePicker.Date = DateTime.Today.AddMonths(-1);
            EndDatePicker.Date = DateTime.Today;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await LoadCategoriesAsync();
            await LoadTransactionsAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                _categories = await _categoryService.GetAllAsync();

                FilterCategoryPicker.ItemsSource = _categories;
                FilterCategoryPicker.ItemDisplayBinding = new Binding(nameof(CategoryListDto.Name));
            }
            catch
            {
                MessageLabel.Text = "Kategoriler yüklenirken hata oluştu.";
            }
        }

        private async Task LoadTransactionsAsync()
        {
            try
            {
                MessageLabel.Text = "İşlemler yükleniyor...";

                var transactions = await _transactionService.GetTransactionsByUserIdAsync(SessionManager.UserId);

                SetTransactionList(transactions);
            }
            catch
            {
                MessageLabel.Text = "İşlemler yüklenirken hata oluştu.";
            }
            finally
            {
                TransactionsRefreshView.IsRefreshing = false;
            }
        }

        private void SetTransactionList(List<TransactionListDto> transactions)
        {
            var userTransactions = transactions
                .Where(transaction => transaction.UserId == SessionManager.UserId)
                .ToList();

            _transactions.Clear();

            foreach (var transaction in userTransactions)
            {
                _transactions.Add(transaction);
            }

            MessageLabel.Text = _transactions.Count == 0
                ? "Kayıt bulunamadı."
                : "";
        }

        private async void OnDateFilterClicked(object sender, EventArgs e)
        {
            if (StartDatePicker.Date > EndDatePicker.Date)
            {
                MessageLabel.Text = "Başlangıç tarihi bitiş tarihinden büyük olamaz.";
                return;
            }

            try
            {
                var transactions = await _transactionService.GetTransactionsByDateRangeAsync(
                    StartDatePicker.Date ?? DateTime.Today,
                    EndDatePicker.Date ?? DateTime.Today);

                SetTransactionList(transactions);
            }
            catch
            {
                MessageLabel.Text = "Tarih filtresi uygulanamadı.";
            }
        }

        private async void OnCategoryFilterClicked(object sender, EventArgs e)
        {
            if (FilterCategoryPicker.SelectedItem is not CategoryListDto selectedCategory)
            {
                MessageLabel.Text = "Kategori seçmelisiniz.";
                return;
            }

            try
            {
                var transactions = await _transactionService.GetTransactionsByCategoryIdAsync(selectedCategory.Id);

                SetTransactionList(transactions);
            }
            catch
            {
                MessageLabel.Text = "Kategori filtresi uygulanamadı.";
            }
        }

        private async void OnIncomeFilterClicked(object sender, EventArgs e)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByTypeAsync(TransactionType.Income);

                SetTransactionList(transactions);
            }
            catch
            {
                MessageLabel.Text = "Gelir filtresi uygulanamadı.";
            }
        }

        private async void OnExpenseFilterClicked(object sender, EventArgs e)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByTypeAsync(TransactionType.Expense);

                SetTransactionList(transactions);
            }
            catch
            {
                MessageLabel.Text = "Gider filtresi uygulanamadı.";
            }
        }

        private async void OnClearFilterClicked(object sender, EventArgs e)
        {
            FilterCategoryPicker.SelectedItem = null;
            StartDatePicker.Date = DateTime.Today.AddMonths(-1);
            EndDatePicker.Date = DateTime.Today;

            await LoadTransactionsAsync();
        }

        private async void OnRefreshing(object sender, EventArgs e)
        {
            await LoadTransactionsAsync();
        }

        private async void OnTransactionSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not TransactionListDto selectedTransaction)
            {
                return;
            }

            NavigationState.SelectedTransaction = selectedTransaction;

            TransactionsCollectionView.SelectedItem = null;

            await Shell.Current.GoToAsync(nameof(TransactionEditPage));
        }
    }
}