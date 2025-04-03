namespace ToDo.Domain.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool Completed { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public Guid UserId { get; set; } 
        public User? User { get; set; } 
    }

}
