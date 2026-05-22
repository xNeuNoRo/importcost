using Microsoft.EntityFrameworkCore;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.EntityConfigurations;

namespace ImportCostPro.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CountryEntityConfiguration());
        } 

    }
}