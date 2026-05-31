using HarcaBak.Mobile.Helpers;
using HarcaBak.Mobile.Models;
using HarcaBak.Mobile.Services;
using System.Globalization;

namespace HarcaBak.Mobile.Views
{
    public partial class TransactionEditPage : ContentPage
    {
        private readonly ICategoryService _categoryService;
        private readonly ITransactionService _transactionService;

        private List<CategoryListDto> _categories = new();
        private TransactionListDto? _transaction;

        public TransactionEditPage()
        {
            InitializeComponent();

            _categoryService = new CategoryService();
            _transactionService = new TransactionService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _transaction = NavigationState.SelectedTransaction;

            if (_transaction == null)
            {
                MessageLabel.Text = "Düzenlenecek işlem bulunamadı.";
                return;
            }

            await LoadCategoriesAsync();

            FillForm();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                _categories = await _categoryService.GetAllAsync();

                CategoryPicker.ItemsSource = _categories;
                CategoryPicker.ItemDisplayBinding = new Binding(nameof(CategoryListDto.Name));
            }
            catch
            {
                MessageLabel.Text = "Kategoriler yüklenirken hata oluştu.";
            }
        }

        private void FillForm()
        {
            if (_transaction == null)
            {
                return;
            }

            AmountEntry.Text = _transaction.Amount.ToString(CultureInfo.CurrentCulture);
            DescriptionEntry.Text = _transaction.Description;
            TransactionDatePicker.Date = _transaction.Date;

            CategoryPicker.SelectedItem = _categories
                .FirstOrDefault(category => category.Id == _transaction.CategoryId);

            IncomeRadioButton.IsChecked = _transaction.Type == TransactionType.Income;
            ExpenseRadioButton.IsChecked = _transaction.Type == TransactionType.Expense;
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            if (_transaction == null)
            {
                MessageLabel.Text = "Düzenlenecek işlem bulunamadı.";
                return;
            }

            if (string.IsNullOrWhiteSpace(AmountEntry.Text))
            {
                MessageLabel.Text = "Tutar boş bırakılamaz.";
                return;
            }

            if (!decimal.TryParse(AmountEntry.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount))
            {
                MessageLabel.Text = "Geçerli bir tutar giriniz.";
                return;
            }

            if (amount <= 0)
            {
                MessageLabel.Text = "Tutar 0'dan büyük olmalıdır.";
                return;
            }

            if (DescriptionEntry.Text != null && DescriptionEntry.Text.Length > 100)
            {
                MessageLabel.Text = "Açıklama en fazla 100 karakter olabilir.";
                return;
            }

            if (CategoryPicker.SelectedItem is not CategoryListDto selectedCategory)
            {
                MessageLabel.Text = "Kategori seçmelisiniz.";
                return;
            }

            var transactionType = IncomeRadioButton.IsChecked
                ? TransactionType.Income
                : TransactionType.Expense;

            var transactionUpdateDto = new TransactionUpdateDto
            {
                Amount = amount,
                Description = DescriptionEntry.Text,
                Date = TransactionDatePicker.Date ?? DateTime.Today,
                Type = transactionType,
                CategoryId = selectedCategory.Id,
                UserId = SessionManager.UserId
            };

            try
            {
                var isSuccess = await _transactionService.UpdateAsync(_transaction.Id, transactionUpdateDto);

                if (!isSuccess)
                {
                    MessageLabel.Text = "İşlem güncellenemedi.";
                    return;
                }

                MessageLabel.Text = "İşlem başarıyla güncellendi.";

                await Shell.Current.GoToAsync(nameof(TransactionListPage));
            }
            catch
            {
                MessageLabel.Text = "API bağlantısı kurulamadı.";
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_transaction == null)
            {
                MessageLabel.Text = "Silinecek işlem bulunamadı.";
                return;
            }

            var confirm = await DisplayAlertAsync(
                "Silme Onayı",
                "Bu işlemi silmek istediğinize emin misiniz?",
                "Evet",
                "Hayır");

            if (!confirm)
            {
                return;
            }

            try
            {
                var isSuccess = await _transactionService.DeleteAsync(_transaction.Id);

                if (!isSuccess)
                {
                    MessageLabel.Text = "İşlem silinemedi.";
                    return;
                }

                NavigationState.SelectedTransaction = null;

                await Shell.Current.GoToAsync(nameof(TransactionListPage));
            }
            catch
            {
                MessageLabel.Text = "API bağlantısı kurulamadı.";
            }
        }
    }
}