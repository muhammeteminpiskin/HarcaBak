using HarcaBak.Mobile.Helpers;
using HarcaBak.Mobile.Models;
using HarcaBak.Mobile.Services;
using System.Collections.ObjectModel;

namespace HarcaBak.Mobile.Views
{
    public partial class CategoryPage : ContentPage
    {
        private readonly ICategoryService _categoryService;

        private readonly ObservableCollection<CategoryListDto> _categories = new();

        private CategoryListDto? _selectedCategory;

        public CategoryPage()
        {
            InitializeComponent();

            _categoryService = new CategoryService();

            CategoriesCollectionView.ItemsSource = _categories;
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
                MessageLabel.Text = "Kategoriler yükleniyor...";

                var categories = await _categoryService.GetAllAsync();

                _categories.Clear();

                foreach (var category in categories)
                {
                    _categories.Add(category);
                }

                MessageLabel.Text = _categories.Count == 0
                    ? "Henüz kategori yok."
                    : "";
            }
            catch
            {
                MessageLabel.Text = "Kategoriler yüklenirken hata oluştu.";
            }
        }

        private void OnCategorySelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not CategoryListDto selectedCategory)
            {
                return;
            }

            _selectedCategory = selectedCategory;

            CategoryNameEntry.Text = selectedCategory.Name;
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CategoryNameEntry.Text))
            {
                MessageLabel.Text = "Kategori adı boş bırakılamaz.";
                return;
            }

            var categoryCreateDto = new CategoryCreateDto
            {
                Name = CategoryNameEntry.Text.Trim(),
                CreatedByUserId = SessionManager.UserId
            };

            try
            {
                var isSuccess = await _categoryService.AddAsync(categoryCreateDto);

                if (!isSuccess)
                {
                    MessageLabel.Text = "Kategori eklenemedi.";
                    return;
                }

                CategoryNameEntry.Text = string.Empty;
                _selectedCategory = null;
                CategoriesCollectionView.SelectedItem = null;

                MessageLabel.Text = "Kategori başarıyla eklendi.";

                await LoadCategoriesAsync();
            }
            catch
            {
                MessageLabel.Text = "API bağlantısı kurulamadı.";
            }
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageLabel.Text = "Güncellenecek kategori seçmelisiniz.";
                return;
            }

            if (string.IsNullOrWhiteSpace(CategoryNameEntry.Text))
            {
                MessageLabel.Text = "Kategori adı boş bırakılamaz.";
                return;
            }

            var categoryUpdateDto = new CategoryUpdateDto
            {
                Name = CategoryNameEntry.Text.Trim(),
                UpdatedByUserId = SessionManager.UserId
            };

            try
            {
                var isSuccess = await _categoryService.UpdateAsync(_selectedCategory.Id, categoryUpdateDto);

                if (!isSuccess)
                {
                    MessageLabel.Text = "Kategori güncellenemedi.";
                    return;
                }

                CategoryNameEntry.Text = string.Empty;
                _selectedCategory = null;
                CategoriesCollectionView.SelectedItem = null;

                MessageLabel.Text = "Kategori başarıyla güncellendi.";

                await LoadCategoriesAsync();
            }
            catch
            {
                MessageLabel.Text = "API bağlantısı kurulamadı.";
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageLabel.Text = "Silinecek kategori seçmelisiniz.";
                return;
            }

            var confirm = await DisplayAlertAsync(
                "Silme Onayı",
                "Bu kategoriyi silmek istediğinize emin misiniz?",
                "Evet",
                "Hayır");

            if (!confirm)
            {
                return;
            }

            try
            {
                var isSuccess = await _categoryService.DeleteAsync(_selectedCategory.Id);

                if (!isSuccess)
                {
                    MessageLabel.Text = "Kategori silinemedi. Bu kategoriye bağlı işlem olabilir.";
                    return;
                }

                CategoryNameEntry.Text = string.Empty;
                _selectedCategory = null;
                CategoriesCollectionView.SelectedItem = null;

                MessageLabel.Text = "Kategori başarıyla silindi.";

                await LoadCategoriesAsync();
            }
            catch
            {
                MessageLabel.Text = "API bağlantısı kurulamadı.";
            }
        }
    }
}