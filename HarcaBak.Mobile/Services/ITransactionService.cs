using HarcaBak.Mobile.Models;

namespace HarcaBak.Mobile.Services
{
    public interface ITransactionService
    {
        Task<TransactionSummaryDto?> GetSummaryByUserIdAsync(int userId);

        Task<List<TransactionListDto>> GetTransactionsByUserIdAsync(int userId);

        Task<List<TransactionListDto>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<List<TransactionListDto>> GetTransactionsByCategoryIdAsync(int categoryId);

        Task<List<TransactionListDto>> GetTransactionsByTypeAsync(TransactionType type);

        Task<bool> AddAsync(TransactionCreateDto transactionCreateDto);

        Task<bool> UpdateAsync(int id, TransactionUpdateDto transactionUpdateDto);

        Task<bool> DeleteAsync(int id);
    }
}