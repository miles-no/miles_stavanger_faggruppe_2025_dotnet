using TaskManager.Common.Models;

namespace TaskManager.Service.Services
{
    public interface ITodoItemService
    {
        Task<IEnumerable<TodoItem>> GetTodoItemsAsync();
        Task<TodoItem> GetTodoItemAsync(int id);
        Task AddTodoItemAsync(TodoItem todoItem);
        Task UpdateTodoItemAsync(TodoItem todoItem);
        Task DeleteTodoItemAsync(int id);
        Task<bool> TodoItemExistsAsync(int id);
    }
}
