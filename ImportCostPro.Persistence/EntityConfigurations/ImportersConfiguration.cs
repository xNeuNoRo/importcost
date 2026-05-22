using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class ImporterConfiguration : IEntityTypeConfiguration<Importer>
    {
        public void Configure(EntityTypeBuilder<Importer> builder)
        {
            // Primary key (PK)
            builder.ToTable("Importers");

            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(b => b.ComercialName).IsRequired().HasMaxLength(150);

            builder.Property(b => b.RNC).IsRequired().HasMaxLength(20);

            builder.Property(b => b.Phone).HasMaxLength(20);

            builder.Property(b => b.Email).IsRequired().HasMaxLength(100);

            builder.Property(b => b.Address).IsRequired().HasMaxLength(250);

            builder.Property(b => b.IsActive).IsRequired().HasDefaultValue(true);
            #endregion

            #region Indexes
            builder.HasIndex(b => b.RNC).IsUnique();

            #endregion

            #region Relationships
            builder.HasOne<Country>()
                .WithMany()
                .HasForeignKey(b => b.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion


            


         
            


            
        }
    }
}
