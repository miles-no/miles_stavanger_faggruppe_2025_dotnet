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

        public DbSet<UserStat> UserStats { get; set; }

        public DbSet<TodoItem> ToDos { get; set; }
    }
}
