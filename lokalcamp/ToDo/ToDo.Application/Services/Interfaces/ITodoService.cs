using ToDo.Business.Dtos;

namespace ToDo.Business.Services.Interfaces
{
    public interface ITodoService
    {
        Task<List<TodoDto>> GetAllAsync();
        Task<TodoDto> GetByIdAsync(int id);
        Task<TodoDto> AddAsync(TodoDto todo);
        Task<TodoDto> UpdateAsync(int id, TodoDto todo);
        Task<bool> DeleteAsync(int id);
    }
}
