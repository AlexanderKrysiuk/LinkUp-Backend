using LinkUpBackend.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkUpBackend.Infrastructure
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public override DbSet<Role> Roles => Set<Role>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Role>().HasData(new List<Role>
            {
                new Role
                {
                    Id = Guid.NewGuid().ToString(), Name = "Client", NormalizedName = "CLIENT"
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(), Name = "Contractor", NormalizedName = "CONTRACTOR"
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(), Name = "Admin", NormalizedName = "ADMIN"
                },
            }
            );
        }
    }
}
