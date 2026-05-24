using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");

            // Primary Key (PK)
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);

            builder.Property(x => x.IsoCode).IsRequired().HasMaxLength(3);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            #endregion

            #region Indexes

            builder.HasIndex(x => x.IsoCode).IsUnique();

            #endregion
        }
    }
}
