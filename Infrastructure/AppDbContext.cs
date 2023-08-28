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
        }
    }
}
