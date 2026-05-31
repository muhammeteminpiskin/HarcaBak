using HarcaBak.Mobile.Helpers;
using HarcaBak.Mobile.Models;
using HarcaBak.Mobile.Services;
using System.Globalization;

namespace HarcaBak.Mobile.Views
{
    public partial class TransactionAddPage : ContentPage
    {
        private readonly ICategoryService _categoryService;
        private readonly ITransactionService _transactionService;

        private List<CategoryListDto> _categories = new();

        public TransactionAddPage()
        {
            InitializeComponent();

            _categoryService = new CategoryService();
            _transactionService = new TransactionService();

            TransactionDatePicker.Date = DateTime.Today;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                _categories = await _categoryService.GetAllAsync();

                CategoryPicker.ItemsSource = _categories;
                CategoryPicker.ItemDisplayBinding = new Binding(nameof(CategoryListDto.Name));

                if (_categories.Count == 0)
                {
                    MessageLabel.Text = "Önce kategori eklemelisiniz.";
                }
            }
            catch
            {
                MessageLabel.Text = "Kategoriler yüklenirken hata oluştu.";
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
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

            var transactionCreateDto = new TransactionCreateDto
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
                var isSuccess = await _transactionService.AddAsync(transactionCreateDto);

                if (!isSuccess)
                {
                    MessageLabel.Text = "İşlem eklenemedi.";
                    return;
                }

                MessageLabel.Text = "İşlem başarıyla eklendi.";

                AmountEntry.Text = string.Empty;
                DescriptionEntry.Text = string.Empty;
                CategoryPicker.SelectedItem = null;
                ExpenseRadioButton.IsChecked = true;
                TransactionDatePicker.Date = DateTime.Today;
            }
            catch
            {
                MessageLabel.Text = "API bağlantısı kurulamadı.";
            }
        }
    }
}