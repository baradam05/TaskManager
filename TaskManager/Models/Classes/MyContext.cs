using Microsoft.EntityFrameworkCore;
using TaskManager.Models.DbClasses;

namespace TaskManager.Models.Classes
{
    public class MyContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<Assignment> Assignment { get; set; }
        public DbSet<SubAssignment> SubAssignment { get; set; }
        public DbSet<Assigned> Assigned { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=TaskManager;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assigned>().HasKey(a => new { a.AccountId, a.AssignmentId });
        }
    }
}
