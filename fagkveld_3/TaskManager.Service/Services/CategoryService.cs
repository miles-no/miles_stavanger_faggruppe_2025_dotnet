using TaskManager.Common.Models;
using TaskManager.Data.Entities;
using TaskManager.Data.Repositories;

namespace TaskManager.Service.Services
{
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var categoryEntities = await categoryRepository.GetCategoriesAsync();
            return categoryEntities.Select(entity => new Category
            {
                Id = entity.Id,
                Name = entity.Name
                // Map other properties as needed
            });
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            var categoryEntity = await categoryRepository.GetCategoryByIdAsync(id);
            return new Category
            {
                Id = categoryEntity.Id,
                Name = categoryEntity.Name
                // Map other properties as needed
            };
        }

        public async Task AddCategoryAsync(CreateCategory category)
        {
            var categoryEntity = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = category.Name
                // Map other properties as needed
            };
            await categoryRepository.AddCategoryAsync(categoryEntity);
        }

        public async Task UpdateCategoryAsync(Guid id, Category category)
        {
            if (id != category.Id)
            {
                throw new ArgumentException("ID mismatch");
            }

            if (!await categoryRepository.CategoryExistsAsync(id))
            {
                throw new KeyNotFoundException("Category not found");
            }

            var categoryEntity = new CategoryEntity
            {
                Id = category.Id,
                Name = category.Name
                // Map other properties as needed
            };

            await categoryRepository.UpdateCategoryAsync(categoryEntity);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            if (!await categoryRepository.CategoryExistsAsync(id))
            {
                throw new KeyNotFoundException("Category not found");
            }

            await categoryRepository.DeleteCategoryAsync(id);
        }
    }
}
