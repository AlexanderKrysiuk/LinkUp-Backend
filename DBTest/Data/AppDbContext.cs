using DBTest.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString: "User Id=linkupadmin; Password=LinkUpDatabase23; Server=localhost; Database=linkupdb; Integrated Security=true;");
            base.OnConfiguring(optionsBuilder);
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>(e => e.ToTable("users"));
        //    modelBuilder.Entity<User>(entity =>
        //    {
        //        entity.Property(e => e.Id)
        //                            .HasColumnName("id")
        //                            .HasDefaultValueSql("nextval('account.item_id_seq'::regclass)");
        //        entity.Property(e => e.Name).IsRequired().HasColumnName("name");
        //        entity.Property(e => e.Email).IsRequired().HasColumnName("email");
        //    });

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
