using TaskManager.Common.Models;
using TaskManager.Data.Entities;
using TaskManager.Data.Repositories;

namespace TaskManager.Service.Services
{
    public class TodoItemService(ITodoItemRepository repository) : ITodoItemService
    {
        public async Task<IEnumerable<TodoItem>> GetTodoItemsAsync()
        {
            var todoItemEntities = await repository.GetTodoItemsAsync();
            return todoItemEntities.Select(entity => new TodoItem
            {
                Id = entity.Id,
                Name = entity.Name,
                IsComplete = entity.IsComplete,
                CategoryId = entity.Category?.Id,
            });
        }

        public async Task<TodoItem> GetTodoItemAsync(int id)
        {
            var entity = await repository.GetTodoItemAsync(id);
            return new TodoItem
            {
                Id = entity.Id,
                Name = entity.Name,
                IsComplete = entity.IsComplete
            };
        }

        public async Task AddTodoItemAsync(TodoItem todoItem)
        {
            var entity = new TodoItemEntity
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete,
                CategoryId = todoItem.CategoryId
            };
            await repository.AddTodoItemAsync(entity);
        }

        public async Task UpdateTodoItemAsync(TodoItem todoItem)
        {
            var entity = new TodoItemEntity
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
            await repository.UpdateTodoItemAsync(entity);
        }

        public async Task DeleteTodoItemAsync(int id)
        {
            await repository.DeleteTodoItemAsync(id);
        }

        public async Task<bool> TodoItemExistsAsync(int id)
        {
            return await repository.TodoItemExistsAsync(id);
        }
    }
}