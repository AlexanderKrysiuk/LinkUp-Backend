using LinkUp.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUp.Database
{
    public class AppContext : DbContext
    {
        public DbSet<ContractorRepository> Contractors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString:
           "Server=localhost;Database=linkupdb;Username=linkupadmin;Password=LinkUpDatabase23;");
            
            base.OnConfiguring(optionsBuilder);
        }
    }
}
