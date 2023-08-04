using LinkUp.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Contractor> Contractors { get; set; }

        public DbSet<Client> Clients { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}
