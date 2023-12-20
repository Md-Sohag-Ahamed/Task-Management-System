using Microsoft.EntityFrameworkCore;

namespace Task_Management.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<tblTask> Tasks { get; set; }
        public DbSet<tblUser> Users { get; set; }

    }
}
