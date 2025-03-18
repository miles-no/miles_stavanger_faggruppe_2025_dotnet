using TaskManager.Common.Models;

namespace TaskManager.Service.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(Guid id);
        Task AddCategoryAsync(CreateCategory category);
        Task UpdateCategoryAsync(Guid id, Category category);
        Task DeleteCategoryAsync(Guid id);
    }
}

