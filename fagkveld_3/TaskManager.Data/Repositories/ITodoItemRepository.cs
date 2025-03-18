using TaskManager.Data.Entities;

namespace TaskManager.Data.Repositories
{
    public interface ITodoItemRepository
    {
        Task<IEnumerable<TodoItemEntity>> GetTodoItemsAsync();
        Task<TodoItemEntity> GetTodoItemAsync(int id);
        Task AddTodoItemAsync(TodoItemEntity todoItem);
        Task UpdateTodoItemAsync(TodoItemEntity todoItem);
        Task DeleteTodoItemAsync(int id);
        Task<bool> TodoItemExistsAsync(int id);
    }
}
