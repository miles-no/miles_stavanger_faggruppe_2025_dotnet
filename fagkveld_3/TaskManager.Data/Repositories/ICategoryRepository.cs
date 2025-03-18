using TaskManager.Data.Entities;

namespace TaskManager.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryEntity>> GetCategoriesAsync();
        Task<CategoryEntity> GetCategoryByIdAsync(Guid id);
        Task AddCategoryAsync(CategoryEntity categoryEntity);
        Task UpdateCategoryAsync(CategoryEntity categoryEntity);
        Task DeleteCategoryAsync(Guid id);
        Task<bool> CategoryExistsAsync(Guid id);
    }
}

