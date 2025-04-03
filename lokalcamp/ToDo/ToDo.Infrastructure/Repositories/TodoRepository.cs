using Microsoft.EntityFrameworkCore;
using ToDo.Data.Repositories.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Data.Repositories
{
    public class TodoRepository(ApplicationDbContext context) : ITodoRepository
    {
        public async Task<TodoItem?> GetAsync(Guid id) 
            => await context.ToDos.FindAsync(id);

        public async Task<List<TodoItem>> GetAllAsync()
            => await context.ToDos.ToListAsync();

        public async Task AddAsync(TodoItem todo)
        {
            await context.ToDos.AddAsync(todo);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TodoItem todo)
        {
            context.ToDos.Update(todo);
            await context.SaveChangesAsync();
        }

        public void Delete(Guid id)
        {
            var todoItem = context.ToDos.First(t => t.Id == id);
            context.ToDos.Remove(todoItem);
        }
    }

}
