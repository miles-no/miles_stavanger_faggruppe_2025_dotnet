using ToDo.Business.Dtos;
using ToDo.Business.Services.Interfaces;
using ToDo.Data.Repositories.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Business.Services
{
    public class TodoService(ITodoRepository repository) : ITodoService
    {
        public async Task<List<TodoDto>> GetAllAsync()
        {
            return (await repository.GetAllAsync()).Select(todo => new TodoDto(todo)).ToList();
        }

        public async Task<TodoDto> AddAsync(TodoDto todo)
        {
            var entity = new TodoItem { Title = todo.Title, Completed = todo.Completed };
            await repository.AddAsync(entity);
            return new TodoDto(entity);
        }

        public async Task<TodoDto?> GetByIdAsync(Guid id)
        {
            var entity = await repository.GetAsync(id);
            return entity != null ? new TodoDto(entity) : null;
        }

        public async Task<TodoDto> UpdateAsync(Guid id, TodoDto todo)
        {
            var entity = new TodoItem { Id = id, Title = todo.Title, Completed = todo.Completed };
            await repository.UpdateAsync(entity);
            return new TodoDto(entity);
        }

        public void Delete(Guid id)
        {
            repository.Delete(id);
        }
    }

}
