using HarcaBak.Mobile.Helpers;
using HarcaBak.Mobile.Models;
using System.Net.Http.Json;

namespace HarcaBak.Mobile.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;

        public TransactionService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiSettings.BaseUrl)
            };
        }

        public async Task<TransactionSummaryDto?> GetSummaryByUserIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/transactions/summary/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TransactionSummaryDto>();
        }

        public async Task<List<TransactionListDto>> GetTransactionsByUserIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/transactions/filter/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<TransactionListDto>();
            }

            var transactions = await response.Content.ReadFromJsonAsync<List<TransactionListDto>>();

            return transactions ?? new List<TransactionListDto>();
        }

        public async Task<List<TransactionListDto>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var startText = startDate.ToString("yyyy-MM-dd");
            var endText = endDate.ToString("yyyy-MM-dd");

            var response = await _httpClient.GetAsync($"/api/transactions/filter/date?startDate={startText}&endDate={endText}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<TransactionListDto>();
            }

            var transactions = await response.Content.ReadFromJsonAsync<List<TransactionListDto>>();

            return transactions ?? new List<TransactionListDto>();
        }

        public async Task<List<TransactionListDto>> GetTransactionsByCategoryIdAsync(int categoryId)
        {
            var response = await _httpClient.GetAsync($"/api/transactions/filter/category/{categoryId}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<TransactionListDto>();
            }

            var transactions = await response.Content.ReadFromJsonAsync<List<TransactionListDto>>();

            return transactions ?? new List<TransactionListDto>();
        }

        public async Task<List<TransactionListDto>> GetTransactionsByTypeAsync(TransactionType type)
        {
            var response = await _httpClient.GetAsync($"/api/transactions/filter/type?type={type}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<TransactionListDto>();
            }

            var transactions = await response.Content.ReadFromJsonAsync<List<TransactionListDto>>();

            return transactions ?? new List<TransactionListDto>();
        }

        public async Task<bool> AddAsync(TransactionCreateDto transactionCreateDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/transactions", transactionCreateDto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, TransactionUpdateDto transactionUpdateDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/transactions/{id}", transactionUpdateDto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/transactions/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}