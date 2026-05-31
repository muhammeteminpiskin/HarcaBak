using HarcaBak.Mobile.Helpers;
using HarcaBak.Mobile.Models;
using System.Net.Http.Json;

namespace HarcaBak.Mobile.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiSettings.BaseUrl)
            };
        }

        public async Task<List<CategoryListDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("/api/categories");

            if (!response.IsSuccessStatusCode)
            {
                return new List<CategoryListDto>();
            }

            var categories = await response.Content.ReadFromJsonAsync<List<CategoryListDto>>();

            return categories ?? new List<CategoryListDto>();
        }

        public async Task<bool> AddAsync(CategoryCreateDto categoryCreateDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/categories", categoryCreateDto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, CategoryUpdateDto categoryUpdateDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/categories/{id}", categoryUpdateDto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/categories/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}