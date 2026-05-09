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
            return _context.Transactions.ToList();
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
            _context.Update(transaction);
            _context.SaveChanges();
        }
    }
}
