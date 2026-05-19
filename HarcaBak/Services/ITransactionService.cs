using HarcaBak.Entities;
namespace HarcaBak.Services
{
    public interface ITransactionService
    {
        void Add(Transaction transaction);
        Transaction? GetById(int id);
        void Delete(int id);
        void Update(Transaction transaction);
        List<Transaction> GetAll();
        List<Transaction> GetByCategoryId(int categoryId);
        List<Transaction> GetByDateRange(DateTime startDate, DateTime endDate);
        List<Transaction> GetByType(TransactionType type);
        List<Transaction> GetByUserId(int userId);
        decimal GetTotalExpenseByUserId(int userId);
        decimal GetTotalIncomeByUserId(int userId);
        decimal GetBalanceByUserId(int userId);
    }
}
