using ToDo.Business.Dtos;

namespace ToDo.Business.Services.Interfaces
{
    public interface ITodoService
    {
        Task<List<TodoDto>> GetAllAsync();
        Task<TodoDto?> GetByIdAsync(Guid id);
        Task<TodoDto> AddAsync(TodoDto todo);
        Task<TodoDto> UpdateAsync(Guid id, TodoDto todo);
        void Delete(Guid id);
    }
}
