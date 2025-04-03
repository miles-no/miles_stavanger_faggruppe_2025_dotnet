using ToDo.Domain.Models;

namespace ToDo.Business.Dtos
{
    public class TodoDto
    {
        public TodoDto(TodoItem todo)
        {
            Id = todo.Id;
            Title = todo.Title;
            Completed = todo.Completed;
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool Completed { get; set; } = false;
    }

}
