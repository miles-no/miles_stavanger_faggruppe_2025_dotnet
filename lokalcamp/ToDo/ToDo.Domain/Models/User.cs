namespace ToDo.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;

        public List<TodoItem> TodoItems { get; set; } = new();

        public UserStat? UserStat { get; set; }
    }
}
