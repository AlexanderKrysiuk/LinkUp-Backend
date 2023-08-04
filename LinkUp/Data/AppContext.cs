using LinkUp.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Data
{
    public class AppContext : DbContext
    {
        public DbSet<Contractor> Contractors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString:
           "Server=localhost;Database=linkupdb;Username=linkupadmin;Password=LinkUpDatabase23;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
