using ToDo.Business.Dtos;
using ToDo.Business.Services.Interfaces;
using ToDo.Data.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Business.Services
{
    public class TodoService(ITodoRepository repository) : ITodoService
    {
        public async Task<List<TodoDto>> GetAllAsync()
            => (await repository.GetAllAsync()).Select(todo => new TodoDto(todo)).ToList();

        public async Task<TodoDto> AddAsync(TodoDto todo)
        {
            var entity = new TodoItem { Title = todo.Title, Completed = todo.Completed };
            await repository.AddAsync(entity);
            return new TodoDto(entity);
        }

        public Task<TodoDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TodoDto> UpdateAsync(int id, TodoDto todo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }

}
