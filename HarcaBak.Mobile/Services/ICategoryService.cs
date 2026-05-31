using HarcaBak.Mobile.Models;

namespace HarcaBak.Mobile.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryListDto>> GetAllAsync();

        Task<bool> AddAsync(CategoryCreateDto categoryCreateDto);

        Task<bool> UpdateAsync(int id, CategoryUpdateDto categoryUpdateDto);

        Task<bool> DeleteAsync(int id);
    }
}