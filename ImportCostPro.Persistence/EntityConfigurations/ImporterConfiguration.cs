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

            builder.Property(x => x.LegalName).IsRequired().HasMaxLength(150);

            builder.Property(x => x.TaxId).IsRequired().HasMaxLength(20);

            builder.Property(x => x.PhoneNumber).HasMaxLength(20);

            builder.Property(x => x.Email).HasMaxLength(100);

            builder.Property(x => x.Address).HasMaxLength(250);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            #endregion

            #region Indexes

            builder.HasIndex(x => x.TaxId).IsUnique();

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
