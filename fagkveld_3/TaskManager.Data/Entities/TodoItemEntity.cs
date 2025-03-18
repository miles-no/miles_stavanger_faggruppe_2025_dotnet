namespace TaskManager.Data.Entities
{
    public class TodoItemEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public CategoryEntity? Category { get; set; }
        public Guid? CategoryId { get; set; }
        public List<CommentEntity>? Comments { get; set; }
    }
}