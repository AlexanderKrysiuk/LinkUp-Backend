using LinkUp.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Contractor> Contractors { get; set; }

        public DbSet<Client> Clients { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contractor>().HasKey(x => x.Id);
            modelBuilder.Entity<Client>().HasKey(x => x.Id);
            base.OnModelCreating(modelBuilder);
        }

    }
}
