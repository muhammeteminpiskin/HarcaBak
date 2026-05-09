using HarcaBak.Entities;
namespace HarcaBak.Services
{
    public interface ICategoryService
    {
        void Add(Category category);
        Category? GetById(int id);
        void Delete(int id);
        void Update(Category category);
        List<Category> GetAll();
    }
}
