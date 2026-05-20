using HarcaBak.Data;
using HarcaBak.Entities;
using Microsoft.EntityFrameworkCore;
namespace HarcaBak.Services
{
    public class TransactionService: ITransactionService
    {
        private readonly AppDbContext _context; 
        public TransactionService(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public void Add(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
        public List<Transaction> GetAll()
        {
            return _context.Transactions
                .Include(x => x.Category)
                .Include(x => x.User)
                .ToList();
        }
        public Transaction? GetById(int id)
        {
            return _context.Transactions
                .Include(x => x.Category)
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == id);
        }
        public void Delete(int id)
        {
            var existingTransaction = GetById(id);
            if (existingTransaction != null)
            {
                _context.Remove(existingTransaction);
                _context.SaveChanges();
            }
        }
        public void Update(Transaction transaction)
        {
            _context.Update(transaction);
            _context.SaveChanges();
        }
        public List<Transaction> GetByCategoryId(int categoryId)
        {
            return _context.Transactions
                .Include(x => x.Category)
                .Include(x => x.User)
                .Where(x => x.CategoryId == categoryId)
                .ToList();
        }
        public List<Transaction> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Transactions
                .Include(x => x.Category)
                .Include(x => x.User)
                .Where(x => x.Date >= startDate && x.Date <= endDate).
                ToList();
        }
        public List<Transaction> GetByType(TransactionType type)
        {
            return _context.Transactions
                .Include(x => x.Category)
                .Include(x => x.User)
                .Where(x => x.Type == type)
                .ToList();
        }
        public List<Transaction> GetByUserId(int userId)
        {
            return _context.Transactions
                .Include(x => x.Category)
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .ToList();
        }
        public decimal GetTotalExpenseByUserId(int userId)
        {
            return _context.Transactions
                .Where(x => x.UserId == userId && x.Type == TransactionType.Expense)
                .Sum(x => x.Amount);
        }
        public decimal GetTotalIncomeByUserId(int userId)
        {
            return _context.Transactions
                .Where(x => x.UserId == userId && x.Type == TransactionType.Income)
                .Sum(x => x.Amount);
        }
        public decimal GetBalanceByUserId(int userId)
        {
            var totalIncome = GetTotalIncomeByUserId(userId);
            var totalExpense = GetTotalExpenseByUserId(userId);

            return totalIncome - totalExpense;
        }
        public bool HasTransactionsByCategoryId(int categoryId)
        {
            return _context.Transactions
                .Any(x => x.CategoryId == categoryId);
        }
        public bool HasTransactionsByUserId(int userId)
        {
            return _context.Transactions
                .Any(x => x.UserId == userId);
        }
    }
}
