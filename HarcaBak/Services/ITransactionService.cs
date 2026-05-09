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
    }
}
