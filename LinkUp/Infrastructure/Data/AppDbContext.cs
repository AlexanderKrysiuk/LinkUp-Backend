using LinkUp.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().Property(x => x.Name).HasColumnType("varchar(40)");
            modelBuilder.Entity<User>().Property(x => x.Email).HasColumnType("varchar(30)");
            modelBuilder.Entity<User>().Property(x => x.Password).HasColumnType("varchar(30)");
            modelBuilder.Entity<User>().Property(x => x.UserType).HasColumnType("varchar(30)");
            //.HasConversion<string>;
            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();

            base.OnModelCreating(modelBuilder);
        }

    }
}
