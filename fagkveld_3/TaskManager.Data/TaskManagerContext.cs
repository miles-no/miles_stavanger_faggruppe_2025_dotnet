using Microsoft.EntityFrameworkCore;
using TaskManager.Data.Entities;

namespace TaskManager.Data
{
    public class TaskManagerContext(DbContextOptions<TaskManagerContext> options) : DbContext(options)
    {
        public DbSet<TodoItemEntity> TodoItems { get; set; } = null!;

        public DbSet<CategoryEntity> Categories { get; set; } = null!;

        public DbSet<CommentEntity> Comments { get; set; } = null!;
    }
}