namespace TaskManager.Data.Entities
{
    public class CommentEntity
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public TodoItemEntity? TodoItem { get; set; }
        public int? TodoItemId { get; set; }
    }
}
