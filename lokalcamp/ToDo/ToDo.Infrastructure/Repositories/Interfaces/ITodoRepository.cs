using ToDo.Domain.Models;

namespace ToDo.Data.Repositories.Interfaces
{
    public interface ITodoRepository
    {
        Task<List<TodoItem>> GetAllAsync();
        Task AddAsync(TodoItem todo);
        Task UpdateAsync(TodoItem todo);
        void Delete(Guid id);
        Task<TodoItem?> GetAsync(Guid id);
    }
}
