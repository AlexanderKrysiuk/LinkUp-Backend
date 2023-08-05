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
            modelBuilder.Entity<Contractor>().Property(x => x.Name).HasColumnType("varchar(30)");
            modelBuilder.Entity<Contractor>().Property(x => x.Email).HasColumnType("varchar(30)");
            modelBuilder.Entity<Contractor>().Property(x => x.Password).HasColumnType("varchar(30)");
            
            modelBuilder.Entity<Client>().HasKey(x => x.Id);
            modelBuilder.Entity<Client>().Property(x => x.Name).HasColumnType("varchar(30)");
            modelBuilder.Entity<Client>().Property(x => x.Email).HasColumnType("varchar(30)");
            modelBuilder.Entity<Client>().Property(x => x.Password).HasColumnType("varchar(30)");

            base.OnModelCreating(modelBuilder);
        }

    }
}
