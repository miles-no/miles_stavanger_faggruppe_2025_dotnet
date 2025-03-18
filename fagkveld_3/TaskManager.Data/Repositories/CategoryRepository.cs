using Microsoft.EntityFrameworkCore;
using TaskManager.Data.Entities;

namespace TaskManager.Data.Repositories
{
    public class CategoryRepository(TaskManagerContext context) : ICategoryRepository
    {
        public async Task<IEnumerable<CategoryEntity>> GetCategoriesAsync()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<CategoryEntity> GetCategoryByIdAsync(Guid id)
        {
            return await context.Categories.FindAsync(id);
        }

        public async Task AddCategoryAsync(CategoryEntity categoryEntity)
        {
            context.Categories.Add(categoryEntity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(CategoryEntity categoryEntity)
        {
            context.Entry(categoryEntity).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var categoryEntity = await context.Categories.FindAsync(id);
            if (categoryEntity != null)
            {
                context.Categories.Remove(categoryEntity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> CategoryExistsAsync(Guid id)
        {
            return await context.Categories.AnyAsync(e => e.Id == id);
        }
    }
}