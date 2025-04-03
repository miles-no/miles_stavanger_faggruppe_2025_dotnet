using Microsoft.EntityFrameworkCore;
using ToDo.Data.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Data.Repositories
{
    public class TodoRepository(ApplicationDbContext context) : ITodoRepository
    {
        public async Task<List<TodoItem>> GetAllAsync()
            => await context.ToDos.ToListAsync();

        public async Task AddAsync(TodoItem todo)
        {
            await context.ToDos.AddAsync(todo);
            await context.SaveChangesAsync();
        }

        public Task UpdateAsync(TodoItem todo)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }

}
