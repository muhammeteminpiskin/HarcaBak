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
            transaction.CreatedDate = DateTime.Now;
            transaction.CreatedBy = 1; // Şuanlık 1 olarak ayarlandı, ilerde dinamikleşecek.
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
        public List<Transaction> GetAll()
        {
            return _context.Transactions.Include(x => x.Category).Include(x => x.User).ToList();
        }
        public Transaction? GetById(int id)
        {
            return _context.Transactions.FirstOrDefault(x => x.Id == id);
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
            var existingTransaction = _context.Transactions.FirstOrDefault(x => x.Id == transaction.Id);
            if (existingTransaction != null)
            {
                existingTransaction.Amount = transaction.Amount;
                existingTransaction.Description = transaction.Description;
                existingTransaction.Type = transaction.Type;
                existingTransaction.CategoryId = transaction.CategoryId;
                existingTransaction.UpdatedDate = DateTime.Now;
                existingTransaction.UpdatedBy = 1; // Şuanlık 1 olarak ayarlandı, ilerde dinamikleşecek.
                _context.SaveChanges();
            }
        }
        public List<Transaction> GetByCategoryId(int categoryId)
        {
            return _context.Transactions.Where(x => x.CategoryId == categoryId).ToList();
        }
        public List<Transaction> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Transactions.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
        }
        public List<Transaction> GetByType(TransactionType type)
        {
            return _context.Transactions.Where(x => x.Type == type).ToList();
        }
    }
}
