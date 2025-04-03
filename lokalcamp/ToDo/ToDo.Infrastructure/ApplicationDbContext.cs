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

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TodoItem> ToDos { get; set; }
    }
}
