using HarcaBak.Data;
using HarcaBak.Entities;
using Microsoft.EntityFrameworkCore;
namespace HarcaBak.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
        public List<Category> GetAll()
        {
            return _context.Categories.ToList();
        }
        public Category? GetById(int id)
        {
            return _context.Categories.FirstOrDefault(x => x.Id == id);
        }
        public void Delete(int id)
        {
            var existingCategory = GetById(id);
            if (existingCategory != null)
            {
                _context.Categories.Remove(existingCategory);
                _context.SaveChanges();
            }
        }
        public void Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }
    }
}
