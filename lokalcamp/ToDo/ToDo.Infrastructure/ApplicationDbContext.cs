using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Models;

namespace ToDo.Data
{
    public class ApplicationDbContext : DbContext
    {
        // for migrations
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserStat> UserStats { get; set; }

        public DbSet<TodoItem> ToDos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.TodoItems)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<UserStat>()
                .HasOne(u => u.User)
                .WithOne(t => t.UserStat);
        }
    }
}
